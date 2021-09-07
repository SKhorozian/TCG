using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using TMPro;
using System;
using MLAPI.Spawning;

public class FieldUnit : FieldCard
{
    UnitCardInstance unitCard;

    NetworkVariableInt power = new NetworkVariableInt(new NetworkVariableSettings{
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.ServerOnly
    });

    NetworkVariableInt movementSpeed = new NetworkVariableInt(new NetworkVariableSettings{
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.ServerOnly
    });

    NetworkVariableInt attackRange = new NetworkVariableInt(new NetworkVariableSettings{
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.ServerOnly
    });

    NetworkVariableBool hasMoved = new NetworkVariableBool(new NetworkVariableSettings{
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.ServerOnly
    });

    [SerializeField] TextMeshPro powerText;
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
        power.Value = unitCard.Power;
        health.Value = unitCard.Health;

        Energize ();

        movementSpeed.Value = unitCard.MovementSpeed;
        attackRange.Value = unitCard.AttackRange;

        CardInstanceInfo cardInfo = new CardInstanceInfo (card.CostChange, unitCard.PowerBonus, unitCard.HealthBonus, unitCard.RangeBonus, unitCard.SpeedBonus, new StaticKeyword[0]);

        SummonUnitClientRPC (card.CardLocation, cardInfo, player.NetworkObjectId, cell.Position);

        //Set Card Effects
        effectTriggers = new CardEffectTrigger [unitCard.UnitCard.CardEffects.Length];
        for (int i = 0; i < unitCard.UnitCard.CardEffects.Length; i++) {
            effectTriggers[i] = Instantiate<CardEffectTrigger> (unitCard.UnitCard.CardEffects[i]);
            effectTriggers[i].FieldCard = this;
        }

        effectListeners = new CardEffectListener [unitCard.UnitCard.CardEffectListeners.Length];
        for (int i = 0; i < unitCard.UnitCard.CardEffectListeners.Length; i++) {
            effectListeners[i] = Instantiate<CardEffectListener> (unitCard.UnitCard.CardEffectListeners[i]);
            effectListeners[i].FieldCard = this;
            effectListeners[i].RegisterListener (player);
        }

        //Set Actions
        actions = new ActionAbility [unitCard.UnitCard.Actions.Count];
        for (int i = 0; i < unitCard.UnitCard.Actions.Count; i++) {
            actions[i] = Instantiate<ActionAbility> (unitCard.UnitCard.Actions[i]);
            actions[i].name = unitCard.UnitCard.Actions[i].name;
            actions[i].FieldCard = this;
        }

        //Call Enterance Effects:
        foreach (CardEffectTrigger effect in effectTriggers) {
            if (effect.Trigger.HasFlag (EffectTrigger.Entrance)) {
                player.MatchManage.AddEffectToStack (effect.GetCardEffect());
            }
        }
    }

    [ClientRpc] 
    void SummonUnitClientRPC (string cardLocation, CardInstanceInfo cardInfo, ulong playerObjectId, Vector2 cellPos) {
        if (!IsServer) {
            unitCard = new UnitCardInstance (Resources.Load<Card> (cardLocation), cardInfo);
            card = unitCard;

            if (NetworkSpawnManager.SpawnedObjects.TryGetValue (playerObjectId, out var playerObject))
                player = playerObject.GetComponent<Player>();

            if (player.MatchManage.FieldGrid.Cells.TryGetValue (cellPos, out var hexCell))
                cell = hexCell;
        } 

        //Render Card Art on object
        //icon.sprite = card.CardArt;

        powerText.text = power.Value.ToString();
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

        if (power.Value <= 0) return; //If this unit has no power, it cannot strike.
        Damage damage = new Damage (power.Value, DamageSource.Attack, player);
        player.DamageTarget (target, damage);

        //player.MatchManage.eventTracker.AddEvent (new StrikeEvent (player, player.MatchManage.TurnNumber, unitCard, ));
    } 

    public void UpdateUnit () {
        transform.position = position.Value;
        powerText.text = power.Value.ToString();
        healthText.text = health.Value.ToString();
        tallyText.text = tallies.Value.ToString ();

        if (IsServer) UpdateUnitClientRPC (cell.Position, card.CostChange, unitCard.PowerBonus, unitCard.HealthBonus, unitCard.RangeBonus, unitCard.SpeedBonus, new StaticKeyword[0]);
    }
    [ClientRpc]
    void UpdateUnitClientRPC (Vector2 cellPos, int costChange, int bonuspower, int bonusHealth, int bonusRange, int bonusSpeed, StaticKeyword[] staticKeywords) {
        if (!IsClient) return;
        if (player == null) {SynchronizeObjectRequestServerRPC (); return;} //Syncronize the object, then return.

        transform.position = position.Value;
        powerText.text = power.Value.ToString();
        healthText.text = health.Value.ToString();
        actionPointText.text = currActionPoints.Value.ToString();
        tallyText.text = tallies.Value.ToString ();

        if (unitCard != null)
            unitCard.SetCardInfo (new CardInstanceInfo (costChange, bonuspower, bonusHealth, bonusRange, bonusSpeed, staticKeywords));

        if (player.MatchManage.FieldGrid.Cells.TryGetValue (cellPos, out var hexCell))
            cell = hexCell;
    }

    [ServerRpc]
    public void SynchronizeObjectRequestServerRPC () {
        CardInstanceInfo cardInfo = new CardInstanceInfo (card.CostChange, unitCard.PowerBonus, unitCard.HealthBonus, unitCard.RangeBonus, unitCard.SpeedBonus, new StaticKeyword[0]);
        SyncronizeObjectClientRPC (card.CardLocation, cardInfo, player.NetworkObjectId, cell.Position);
    }
    [ClientRpc]
    public void SyncronizeObjectClientRPC (string cardLocation, CardInstanceInfo cardInfo, ulong playerObjectId, Vector2 cellPos) {
        if (IsServer) return; //We don't want this to run on the server.

        unitCard = new UnitCardInstance (Resources.Load<Card> (cardLocation), cardInfo);
        card = unitCard;

        if (NetworkSpawnManager.SpawnedObjects.TryGetValue (playerObjectId, out var playerObject))
            player = playerObject.GetComponent<Player>();

        if (player.MatchManage.FieldGrid.Cells.TryGetValue (cellPos, out var hexCell))
            cell = hexCell;

        //Color to show ownership
        if (IsOwner) {
            icon.color = Color.green;
        } else {
            icon.color = Color.red;
        }

        UpdateUnit ();
    }

    public override void Energize () {
        if (!IsServer) return;

        currActionPoints.Value = unitCard.UnitCard.StaticKeywords.Contains (StaticKeyword.DoubleAction) ? 2: 1;
        hasMoved.Value = false;
    }

    public override void TurnStart()
    {
        if (!IsServer) return;

        //If this is Decaying, kill it at the start of round.
        if (unitCard.UnitCard.StaticKeywords.Contains (StaticKeyword.Decaying)) player.UnitToDie (this);

        Energize ();
    
        foreach (StatusEffect status in unitCard.StatusEffects) {
            if (status.Duration == StatusDuration.TurnStart) {
                unitCard.RemoveStatusEffect (status);
                status.RemoveStatus (this);
            }
        }
        unitCard.StatusEffects.RemoveAll (status => status.Duration == StatusDuration.TurnStart);

        //Call Enterance Effects:
        foreach (CardEffectTrigger effect in effectTriggers) {
            if (effect.Trigger.HasFlag (EffectTrigger.TurnStart)) {
                player.MatchManage.AddEffectToStack (effect.GetCardEffect());
            }
        }
    
    }

    public override void TurnEnd () {
        if (!IsServer) return;

        foreach (StatusEffect status in unitCard.StatusEffects) {
            if (status.Duration == StatusDuration.TurnEnd) {
                unitCard.RemoveStatusEffect (status);
                status.RemoveStatus (this);
            }
        }
        unitCard.StatusEffects.RemoveAll (status => status.Duration == StatusDuration.TurnEnd);

        //Call Enterance Effects:
        foreach (CardEffectTrigger effect in effectTriggers) {
            if (effect.Trigger.HasFlag (EffectTrigger.TurnEnd)) {
                player.MatchManage.AddEffectToStack (effect.GetCardEffect());
            }
        }
    }

    public void GiveStats (int power, int health, int range, int speed) {
        this.power.Value += power;
        this.health.Value += health;
        this.attackRange.Value += range;
        this.movementSpeed.Value += speed;
    }


    void Update () {
        if (IsClient) {
            UpdateUnit ();
        }
    }

    public override void OnRemove () {
        if (!IsServer) return;

        toBeRemoved.Value = true;

        for (int i = 0; i < effectListeners.Length; i++) { //Remove all listener effects
            effectListeners[i].RemoveListener (player);
        }

        //Remove all non-permanent Effects
        foreach (StatusEffect status in unitCard.StatusEffects) {
            if (status.Duration != StatusDuration.Permanent) {
                unitCard.RemoveStatusEffect (status);
            }
        }
        unitCard.StatusEffects.RemoveAll (status => status.Duration == StatusDuration.TurnEnd || status.Duration == StatusDuration.TurnStart);

    }

    public override Damage TakeDamage(Damage damageInfo)
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

        UpdateUnit ();

        return damageInfo;
    }

    public override Heal TakeHeal(Heal healInfo)
    {
        if (!IsServer) return null;

        health.Value += healInfo.HealAmount;
        health.Value = Mathf.Clamp (health.Value, 0, unitCard.Health);

        UpdateUnit ();

        return healInfo;
    }

    public new CardInstance Card {get {return unitCard;}}
    public UnitCardInstance UnitsCard {get {return unitCard;}}

    public int Power     {get {return power.Value;}}
    public int Range     {get {return attackRange.Value;}}
    public int Speed     {get {return movementSpeed.Value;}}
    public bool HasMoved {get {return hasMoved.Value;} set {hasMoved.Value = true;}}

}
