using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using TMPro;
using MLAPI.Messaging;
using MLAPI.Spawning;

public class FieldHero : FieldCard, IDamageable, ITargetable
{
    HeroCardInstance heroCard;

    [SerializeField] TextMeshPro healthText;

    public NetworkVariableInt health = new NetworkVariableInt(new NetworkVariableSettings{
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.ServerOnly
    });

    public void SummonHero (CardInstance card, Player player, HexagonCell cell) {
        if (!IsServer) return;

        if (card.Type != CardType.Hero) return; 

        this.card = card;

        //Check if this is a hero first.
        if (card is HeroCardInstance)
            heroCard = card as HeroCardInstance;
        else {
            throw new System.Exception ("CardType doesn't match the card's subclass");
        }

        this.player = player; 
        
        this.cell = cell;

        health.Value = 3000;

        SummonHeroClientRPC (card.CardLocation, player.NetworkObjectId, cell.Position);

        //Call Enterance Effects:
    }

    [ClientRpc] 
    void SummonHeroClientRPC (string cardLocation, ulong playerObjectId, Vector2 cellPos) {
        if (!IsServer) { //If we're the client rotate, the object to match the client's camera
            heroCard = new HeroCardInstance (Resources.Load<Card> (cardLocation));
            card = heroCard;

            if (NetworkSpawnManager.SpawnedObjects.TryGetValue (playerObjectId, out var playerObject)) {
                Debug.Log ("found player");
                player = playerObject.GetComponent<Player>();
                Debug.Log (player);
            }

            if (player.MatchManage.FieldGrid.Cells.TryGetValue (cellPos, out var hexCell))
                cell = hexCell;
        } 

        //Render Card Art on object
        icon.sprite = card.CardArt;

        healthText.text = health.Value.ToString();

        //Color to show ownership
        if (IsOwner) {
            icon.color = Color.green;
        } else {
            icon.color = Color.red;
        }
    }
    

    public void UpdateHero () {
        transform.position = position.Value;
        healthText.text = health.Value.ToString();

        if (IsServer) UpdateHeroClientRPC ();
    }
    [ClientRpc]
    void UpdateHeroClientRPC () {
        transform.position = position.Value;
        healthText.text = health.Value.ToString();
    }

    void Update () {
        if (IsClient) {
            UpdateHero ();
        }
    }

    public override void TurnEnd()
    {
        
    }

    public override void TurnStart()
    {
        
    }

    public Damage TakeDamage(Damage damageInfo)
    {
        if (!IsServer) return null;

        health.Value -= damageInfo.DamageAmount;
        
        health.Value = Mathf.Clamp (health.Value, 0, 3000);

        if (health.Value <= 0) {
            //End this game
        }

        UpdateHeroClientRPC ();

        return damageInfo;
    }

    public Heal TakeHeal(Heal healInfo)
    {
        if (!IsServer) return null;

        health.Value += healInfo.HealAmount;
        
        health.Value = Mathf.Clamp (health.Value, 0, 3000);

        UpdateHeroClientRPC ();

        return healInfo;
    }

    public HeroCardInstance HeroCard {get {return heroCard;}}
}
