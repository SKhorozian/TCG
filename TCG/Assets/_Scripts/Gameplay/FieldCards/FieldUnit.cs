using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using TMPro;
using System;
using MLAPI.Spawning;

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

        SummonUnitClientRPC (card.CardLocation, player.NetworkObjectId, cell.Position);

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
    }

    [ClientRpc] 
    void SummonUnitClientRPC (string cardLocation, ulong playerObjectId, Vector2 cellPos) {
        if (!IsServer) {
            unitCard = new UnitCardInstance (Resources.Load<Card> (cardLocation));
            card = unitCard;

            if (NetworkSpawnManager.SpawnedObjects.TryGetValue (playerObjectId, out var playerObject))
                player = playerObject.GetComponent<Player>();

            if (player.MatchManage.FieldGrid.Cells.TryGetValue (cellPos, out var hexCell))
                cell = hexCell;
        } 

        //Render Card Art on object
        //icon.sprite = card.CardArt;

        strengthText.text = strength.Value.ToString();
        healthText.text = health.Value.ToString();

        //Color to show ownership
        if (IsOwner) {
            icon.color = Color.green;
        } else {
            icon.color = Color.red;
        }

        UpdateUnit ();
    }

    //Unit's basic movement.
    public void MoveUnit (Vector2[] targets, ulong netid) {
        if (IsServer) {
            player.MatchManage.MoveUnit (this, targets, netid);
        } else {
            MoveUnitServerRPC (targets, netid);
        }
    }
    [ServerRpc]
    public void MoveUnitServerRPC (Vector2[] targets, ulong netid) {
        MoveUnit (targets, netid);
    }

    //Unit's Basic attack
    public void Attack (Vector2[] targets, ulong netid) {
        if (IsServer) {
            player.MatchManage.UnitAttack (this, targets, netid);
        } else {
            AttackServerRPC (targets, netid);
        }
    }
    [ServerRpc]
    public void AttackServerRPC (Vector2[] targets, ulong netid) {
        Attack (targets, netid);
    }

    public void Strike (IDamageable target) {
        if (!IsServer) return;

        if (strength.Value <= 0) return; //If this unit has no strength, it cannot strike.
        Damage damage = new Damage (strength.Value, DamageSource.Attack, player);
        player.DamageTarget (target, damage);

        //player.MatchManage.eventTracker.AddEvent (new StrikeEvent (player, player.MatchManage.TurnNumber, unitCard, ));
    } 

    public void UpdateUnit () {
        transform.position = position.Value;
        strengthText.text = strength.Value.ToString();
        healthText.text = health.Value.ToString();

        if (IsServer) UpdateUnitClientRPC (cell.Position);
    }
    [ClientRpc]
    void UpdateUnitClientRPC (Vector2 cellPos) {
        if (!IsClient) return;
        transform.position = position.Value;
        strengthText.text = strength.Value.ToString();
        healthText.text = health.Value.ToString();
        actionPointText.text = currActionPoints.Value.ToString();

        if (player.MatchManage.FieldGrid.Cells.TryGetValue (cellPos, out var hexCell))
            cell = hexCell;
    }

    public void ConsumeActionPoint (int amount) {
        if (!IsServer) return;
        currActionPoints.Value -= amount;
        
        currActionPoints.Value = Mathf.Clamp (currActionPoints.Value, 0, unitCard.ActionPoints);
    }

    public override void TurnStart()
    {
        if (!IsServer) return;

        currActionPoints.Value = unitCard.ActionPoints; //Untap
    
        //Call Enterance Effects:
        foreach (CardEffectTrigger effect in effectTriggers) {
            if (effect.Trigger.HasFlag (EffectTrigger.TurnStart)) {
                player.MatchManage.AddEffectToStack (effect.GetCardEffect());
            }
        }
    
    }

    public override void TurnEnd () {
        if (!IsServer) return;

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

        UpdateUnitClientRPC (cell.Position);

        return damageInfo;
    }

    public Heal TakeHeal(Heal healInfo)
    {
        if (!IsServer) return null;

        health.Value += healInfo.HealAmount;
        health.Value = Mathf.Clamp (health.Value, 0, unitCard.Health);

        UpdateUnitClientRPC (cell.Position);

        return healInfo;
    }

    public new CardInstance Card {get {return unitCard;}}
    public UnitCardInstance UnitsCard {get {return unitCard;}}

}
