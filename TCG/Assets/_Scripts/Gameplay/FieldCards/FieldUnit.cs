using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using TMPro;
using System;

public class FieldUnit : FieldCard, IDamageable
{
    UnitCardInstance unitCard;

    public NetworkVariableInt strength = new NetworkVariableInt(new NetworkVariableSettings{
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.ServerOnly
    });

    public NetworkVariableInt health = new NetworkVariableInt(new NetworkVariableSettings{
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.ServerOnly
    });

    public NetworkVariableInt currActionPoints = new NetworkVariableInt(new NetworkVariableSettings{
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.ServerOnly
    });

    public NetworkVariableInt movementSpeed = new NetworkVariableInt(new NetworkVariableSettings{
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.ServerOnly
    });

    public NetworkVariableInt attackRange = new NetworkVariableInt(new NetworkVariableSettings{
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.ServerOnly
    });


    [SerializeField] TextMeshPro strengthText;
    [SerializeField] TextMeshPro healthText;
    [SerializeField] TextMeshPro actionPointText;


    public void SummonUnit (CardInstance card, Player player, HexagonCell cell) {
        if (!IsServer) return;

        if (card.Type != CardType.Unit) return; 

        this.card = card;

        //Check if this is a unit first.
        if (card.Card is UnitCard)
            unitCard = card as UnitCardInstance;
        else {
            throw new System.Exception ("CardType doesn't match the card's subclass");
        }

        this.player = player; 
        
        this.cell = cell;

        //Initialize stats
        strength.Value = unitCard.Strength;

        health.Value = unitCard.Health;

        currActionPoints.Value = unitCard.ActionPoints;

        movementSpeed.Value = unitCard.MovementSpeed;
        attackRange.Value = unitCard.AttackRange;

        //Set Card Effects
        effectTriggers = new CardEffectTrigger [unitCard.UnitCard.CardEffects.Length];
        for (int i = 0; i < unitCard.UnitCard.CardEffects.Length; i++) {
            effectTriggers[i] = Instantiate<CardEffectTrigger> (unitCard.UnitCard.CardEffects[i]);
            effectTriggers[i].FieldCard = this;
        }

        //Call Enterance Effects:
        foreach (CardEffectTrigger effect in effectTriggers) {
            if (effect.Trigger.HasFlag (EffectTrigger.Entrance)) {
                player.MatchManage.AddEffectToStack (effect.GetCardEffect());
            }
        }

        SummonUnitClientRPC (card.CardLocation, player.OwnerClientId);
    }

    [ClientRpc] 
    void SummonUnitClientRPC (string cardLocation, ulong playerOwnerID) {
        if (!IsServer) { //If we're the client rotate, the object to match the client's camera
            transform.Rotate (new Vector3 (0,180,0));
            card = new UnitCardInstance (Resources.Load<Card> (cardLocation));
        } 

        //Render Card Art on object
        icon.sprite = card.CardArt;

        strengthText.text = strength.Value.ToString();
        healthText.text = health.Value.ToString();

        //Color to show ownership
        if (IsOwner) {
            icon.color = Color.green;
        } else {
            icon.color = Color.red;
        }
    }

    public void TargetCell (Vector2 cell, ulong netid) {
        if (IsServer) {
            HexagonCell hexagonCell;
            if (player.MatchManage.FieldGrid.Cells.TryGetValue (cell, out hexagonCell)) {
                if (hexagonCell.FieldCard) {if (hexagonCell.FieldCard is IDamageable) Attack (cell, netid);} //If the hex cell is ocupied and the occupant is an IDamagable, attack it.
                else MoveUnit (cell, netid); //else just move there
            }
        } else {
            TargetCellServerRPC (cell, netid);
        }
    }
    [ServerRpc]
    public void TargetCellServerRPC (Vector2 cell, ulong netid) {
        TargetCell(cell, netid);
    }

    //Unit's basic movement.
    public void MoveUnit (Vector2 cell, ulong netid) {
        if (!IsServer) return;
        player.MatchManage.MoveUnit (this, cell, netid);
    }

    //Unit's Basic attack
    public void Attack (Vector2 cell, ulong netid) {
        if (!IsServer) return;
        player.MatchManage.UnitAttack (this, cell, netid);
    }

    public void Strike (IDamageable target) {
        Damage damage = new Damage (strength.Value, DamageSource.Attack, player);
        player.DamageTarget (target, damage, this);
    } 

    public void UpdateUnit () {
        transform.position = position.Value;
        strengthText.text = strength.Value.ToString();
        healthText.text = health.Value.ToString();

        if (IsServer) UpdateUnitClientRPC ();
    }
    [ClientRpc]
    void UpdateUnitClientRPC () {
        transform.position = position.Value;
        strengthText.text = strength.Value.ToString();
        healthText.text = health.Value.ToString();
        actionPointText.text = currActionPoints.Value.ToString();
    }

    public void ConsumeActionPoint (int amount) {
        if (!IsServer) return;
        currActionPoints.Value -= amount;
        
        currActionPoints.Value = Mathf.Clamp (currActionPoints.Value, 0, unitCard.ActionPoints);
    }

    public override void TurnStart()
    {
        currActionPoints.Value = unitCard.ActionPoints; //Untap
    
        //Call Enterance Effects:
        foreach (CardEffectTrigger effect in effectTriggers) {
            if (effect.Trigger.HasFlag (EffectTrigger.TurnStart)) {
                player.MatchManage.AddEffectToStack (effect.GetCardEffect());
            }
        }
    
    }

    public override void TurnEnd () {

        //Call Enterance Effects:
        foreach (CardEffectTrigger effect in effectTriggers) {
            if (effect.Trigger.HasFlag (EffectTrigger.TurnEnd)) {
                player.MatchManage.AddEffectToStack (effect.GetCardEffect());
            }
        }

    }


    void Update () {
        if (IsClient) {
            UpdateUnit ();
        }
    }

    public Damage TakeDamage(Damage damageInfo)
    {
        if (!IsServer) return null;

        health.Value -= damageInfo.DamageAmount;
        
        health.Value = Mathf.Clamp (health.Value, 0, unitCard.Health);

        if (health.Value <= 0) {
            player.UnitToDie (this);

            //Call OnDestroy Effects
            foreach (CardEffectTrigger effect in effectTriggers) {
                if (effect.Trigger.HasFlag (EffectTrigger.OnDestroy)) {
                    player.MatchManage.AddEffectToStack (effect.GetCardEffect());
                }
            }
        } else {
            //Call Damage Survive effects
            foreach (CardEffectTrigger effect in effectTriggers) {
                if (effect.Trigger.HasFlag (EffectTrigger.DamageTaken)) {
                    player.MatchManage.AddEffectToStack (effect.GetCardEffect());
                }
            }
        }

        UpdateUnitClientRPC ();

        return damageInfo;
    }

    public Heal TakeHeal(Heal healInfo)
    {
        if (!IsServer) return null;

        health.Value += healInfo.HealAmount;
        health.Value = Mathf.Clamp (health.Value, 0, unitCard.Health);

        UpdateUnitClientRPC ();

        return healInfo;
    }

    public new CardInstance Card {get {return unitCard;}}
    public CardInstance UnitsCard {get {return unitCard;}}

}
