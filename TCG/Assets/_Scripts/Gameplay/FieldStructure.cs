using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using TMPro;

public class FieldStructure : FieldCard
{
    StructureCardInstance structureCard;

    public void SummonStructure (CardInstance card, Player player, HexagonCell cell) {
        if (!IsServer) return;
        if (card.Type != CardType.Structure) return;

        this.card = card;

        if (card.Card is StructureCard)
            structureCard = card as StructureCardInstance;
        else {
            throw new System.Exception ("CardType doesn't match the card's subclass");
        }

        this.player = player;

        this.cell = cell;

        foreach (CardEffect effect in structureCard._StructureCard.EnteranceEffects) {
            player.MatchManage.AddEffectToStack (effect, this);
        }

        SummonStructureClientRPC (card.CardLocation, player.OwnerClientId);
    }

    [ClientRpc] 
    void SummonStructureClientRPC (string cardLocation, ulong playerOwnerID) {
        if (!IsServer) { //If we're the client, rotate the object to match the client's camera
            transform.Rotate (new Vector3 (0,180,0));
            card = new StructureCardInstance (Resources.Load<Card> (cardLocation));
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
        foreach (CardEffect effect in structureCard._StructureCard.TurnStartEffects) { //Upkeep
            player.MatchManage.AddEffectToStack (effect, this);
        }
    }

    public override void TurnEnd()
    {
        foreach (CardEffect effect in structureCard._StructureCard.TurnEndEffects) { //End Turn
            player.MatchManage.AddEffectToStack (effect, this);
        }
    }

    void Update () {
        if (IsClient) {
            UpdateStructure ();
        }
    }

    public new CardInstance Card {get {return structureCard;}}
    public CardInstance StructursCard {get {return structureCard;}}
}
