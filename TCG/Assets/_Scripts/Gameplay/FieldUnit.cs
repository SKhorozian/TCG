using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using TMPro;

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
    
    public NetworkVariableInt maxHealth = new NetworkVariableInt(new NetworkVariableSettings{
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.ServerOnly
    });

    public NetworkVariableInt actionPoints = new NetworkVariableInt(new NetworkVariableSettings{
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.ServerOnly
    });

    [SerializeField] SpriteRenderer icon;
    [SerializeField] TextMeshPro strengthText;
    [SerializeField] TextMeshPro healthText;


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
        
        //Spend Mana
        player.SpendMana (unitCard.Cost);

        //Initialize stats
        strength.Value = unitCard.Strength;
        maxHealth.Value = unitCard.Health;
        health.Value = maxHealth.Value;
        actionPoints.Value = unitCard.ActionPoints;

        //Call Enterance Effects:
        foreach (CardEffect effect in unitCard._UnitCard.EnteranceEffects) {
            effect.DoEffect (this);
        }

        SummonUnitClientRPC (card.CardLocation, player.OwnerClientId);
    }

    [ClientRpc] 
    void SummonUnitClientRPC (string cardLocation, ulong playerOwnerID) {
        if (!IsServer) { //If we're the client rotate the object to match the client's camera
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

    //Unit's basic movement.
    public void MoveUnit (Vector2 cell, ulong netid) {
        if (IsServer) {
            player.MatchManage.MoveUnit (this, cell, netid);
        } else {
            MoveUnitServerRPC (cell, NetworkManager.Singleton.LocalClientId);
        }
    }
    [ServerRpc]
    void MoveUnitServerRPC (Vector2 cell, ulong netid) {
        MoveUnit (cell, netid);
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
    }

    public void Die () {
        if (!IsServer) return;
        player.UnitDie (this);
        NetworkObject.Despawn (true);
    }


    void Update () {
        if (IsClient) {
            UpdateUnit ();
        }
    }

    Damage IDamageable.TakeDamage(Damage damageInfo)
    {
        if (!IsServer) return null;

        health.Value -= damageInfo.DamageAmount;
        
        health.Value = Mathf.Clamp (health.Value, 0, maxHealth.Value);

        if (health.Value <= 0) {
            Die ();
        }

        UpdateUnitClientRPC ();

        return damageInfo;
    }

    public CardInstance UnitsCard {get {return unitCard;}}

}
