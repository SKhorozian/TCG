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

        SummonStructureClientRPC (card.CardLocation, player.NetworkObjectId, cell.Position);

        //Set Card Effects
        effectTriggers = new CardEffectTrigger [structureCard.StructureCard.CardEffects.Length];
        for (int i = 0; i < structureCard.StructureCard.CardEffects.Length; i++) {
            effectTriggers[i] = Instantiate<CardEffectTrigger> (structureCard.StructureCard.CardEffects[i]);
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

        if (IsServer) UpdateStructureClientRPC ();
    }
    [ClientRpc]
    void UpdateStructureClientRPC () {
        transform.position = position.Value;
    }

    public override void TurnStart()
    {
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

    void Update () {
        if (IsClient) {
            UpdateStructure ();
        }
    }

    public new CardInstance Card {get {return structureCard;}}
    public StructureCardInstance StructursCard {get {return structureCard;}}
}
