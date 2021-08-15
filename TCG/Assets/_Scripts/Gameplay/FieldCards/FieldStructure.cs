using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using TMPro;
using MLAPI.Spawning;

public class FieldStructure : FieldCard
{
    StructureCardInstance structureCard;

    public void SummonStructure (CardInstance card, Player player, HexagonCell cell) {
        if (!IsServer) return;
        if (card.Type != CardType.Structure) return;

        this.card = card;

        //Check if this is a structure first
        if (card.Card is StructureCard)
            structureCard = card as StructureCardInstance;
        else {
            throw new System.Exception ("CardType doesn't match the card's subclass");
        }

        this.player = player;

        this.cell = cell;

        this.health.Value = structureCard.Health;

        Energize ();

        SummonStructureClientRPC (card.CardLocation, player.NetworkObjectId, cell.Position);

        //Set Card Effects
        effectTriggers = new CardEffectTrigger [structureCard.StructureCard.CardEffects.Length];
        for (int i = 0; i < structureCard.StructureCard.CardEffects.Length; i++) {
            effectTriggers[i] = Instantiate<CardEffectTrigger> (structureCard.StructureCard.CardEffects[i]);
            effectTriggers[i].FieldCard = this;
        }

        effectListeners = new CardEffectListener [structureCard.StructureCard.CardEffectListeners.Length];
        for (int i = 0; i < structureCard.StructureCard.CardEffectListeners.Length; i++) {
            effectListeners[i] = Instantiate<CardEffectListener> (structureCard.StructureCard.CardEffectListeners[i]);
            effectListeners[i].FieldCard = this;
            effectListeners[i].RegisterListener (player);
        }

        //Set Actions
        actions = new ActionAbility [structureCard.StructureCard.Actions.Count];
        for (int i = 0; i < structureCard.StructureCard.Actions.Count; i++) {
            actions[i] = Instantiate<ActionAbility> (structureCard.StructureCard.Actions[i]);
            actions[i].name = structureCard.StructureCard.Actions[i].name;
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
    void SummonStructureClientRPC (string cardLocation, ulong playerObjectId, Vector2 cellPos) {
        if (!IsServer) { //If we're the client, rotate the object to match the client's camera
            structureCard = new StructureCardInstance (Resources.Load<Card> (cardLocation));
            card = structureCard;

            if (NetworkSpawnManager.SpawnedObjects.TryGetValue (playerObjectId, out var playerObject))
                player = playerObject.GetComponent<Player>();

            if (player.MatchManage.FieldGrid.Cells.TryGetValue (cellPos, out var hexCell))
                cell = hexCell;
        } 

        //Render Card Art on object
        icon.sprite = card.CardArt;

        //Color to show ownership
        if (IsOwner) {
            icon.color = Color.green;
        } else {
            icon.color = Color.red;
        }
    }

    public void UpdateStructure () {
        transform.position = position.Value;
        tallyText.text = tallies.Value.ToString();
        healthText.text = health.Value.ToString ();

        if (IsServer) UpdateStructureClientRPC ();
    }
    [ClientRpc]
    void UpdateStructureClientRPC () {
        transform.position = position.Value;
        tallyText.text = tallies.Value.ToString();
        healthText.text = health.Value.ToString ();
    }

    public override void TurnStart()
    {
        Energize ();

        //Call Enterance Effects:
        foreach (CardEffectTrigger effect in effectTriggers) {
            if (effect.Trigger.HasFlag (EffectTrigger.TurnStart)) {
                player.MatchManage.AddEffectToStack (effect.GetCardEffect());
            }
        }
    }

    public override void TurnEnd()
    {
        //Call Enterance Effects:
        foreach (CardEffectTrigger effect in effectTriggers) {
            if (effect.Trigger.HasFlag (EffectTrigger.TurnEnd)) {
                player.MatchManage.AddEffectToStack (effect.GetCardEffect());
            }
        }
    }

    public override void OnRemove()
    {
        if (!IsServer) return;

        toBeRemoved.Value = true;

        for (int i = 0; i < effectListeners.Length; i++) { //Remove all listener effects
            effectListeners[i].RemoveListener (player);
        }
    }

    void Update () {
        if (IsClient) {
            UpdateStructure ();
        }
    }

    public override void Energize()
    {
        currActionPoints.Value = 1;
    }

    public override Damage TakeDamage(Damage damageInfo)
    {
        if (!IsServer) return null;

        health.Value -= damageInfo.DamageAmount;
        
        health.Value = Mathf.Clamp (health.Value, 0, structureCard.Health);

        if (health.Value <= 0) {
            player.StructureToDemolish (this);

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

        UpdateStructure ();

        return damageInfo;
    }

    public override Heal TakeHeal(Heal healInfo)
    {
        if (!IsServer) return null;

        health.Value += healInfo.HealAmount;
        health.Value = Mathf.Clamp (health.Value, 0, structureCard.Health);

        UpdateStructure ();

        return healInfo;
    }

    public new CardInstance Card {get {return structureCard;}}
    public StructureCardInstance StructursCard {get {return structureCard;}}
}
