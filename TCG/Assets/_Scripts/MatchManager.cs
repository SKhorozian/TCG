 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;


public class MatchManager : NetworkBehaviour
{   
    [SerializeField] Player player1;
    [SerializeField] Player player2;

    //Match Info
    [SerializeField] NetworkVariableInt turnNumber = new NetworkVariableInt (new NetworkVariableSettings {
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.ServerOnly
    });

    [SerializeField] Player playerTurn; //Who's turn is it to play.
    [SerializeField] bool localPlayerTurn; //Does the local client have the priority

    [SerializeField] GameObject fieldGridPrefab;
    HexagonGrid fieldGrid;

    [SerializeField] GameObject fieldUnitPrefab;

    [SerializeField] Stack<CardEffect> effectStack = new Stack<CardEffect> ();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeMatch () {
        if (!IsOwnedByServer) DestroyImmediate (this.gameObject);

        //Get reference for both players
        player1 = NetworkManager.Singleton.ConnectedClientsList[0].PlayerObject.GetComponent<Player>();
        player2 = NetworkManager.Singleton.ConnectedClientsList[1].PlayerObject.GetComponent<Player>();

        player1.MatchManage = this;
        player2.MatchManage = this;

        turnNumber.Value = 1;

        playerTurn = player1;
        localPlayerTurn = true;

        playerTurn.RampManaPermanent (1, true);

        InitializeMatchClientRpc ();
    }
    [ClientRpc]
    void InitializeMatchClientRpc () {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkedClient))
        {
            var player = networkedClient.PlayerObject.GetComponent<Player>();
            if (player)
            {
                player.MatchManage = this;
            }
        }

        fieldGrid = Instantiate (fieldGridPrefab).GetComponent<HexagonGrid> ();
        fieldGrid.InitializeGrid();
        if (!IsOwner) {
            Camera.main.transform.position = (new Vector3 (0,7,5));
            Camera.main.transform.rotation = Quaternion.Euler (new Vector3 (60,-180,0));
        }
    }

    //Returns true if the card was played successfully, else return false
    public bool PlayCard (CardInstance card, Player player, Vector2 hexCellPos) {
        if (!IsServer) return false;

        if (!player.Equals(playerTurn)) return false;
        if (player.CurrentMana < card.Cost) return false;

        switch (card.Type) {
            case CardType.Unit:
                Debug.Log ("Player " + player.OwnerClientId + " played " + card.CardName + " on cell " + hexCellPos);
                HexagonCell cell;
                fieldGrid.Cells.TryGetValue(hexCellPos, out cell);
                if (cell.FieldCard) return false; //If the cell is occupied return false
                SummonUnit (card, player, cell);
                break;
            default:
                break;
        }

        return true;
    }

    void SummonUnit (CardInstance card, Player player, HexagonCell cell) {
        if (!IsServer) return;

        GameObject newUnit = Instantiate(fieldUnitPrefab, Vector3.zero, Quaternion.identity);
        newUnit.gameObject.GetComponent<NetworkObject>().SpawnWithOwnership ( player.OwnerClientId, null, true);
        FieldUnit fieldUnit = newUnit.GetComponent<FieldUnit> ();
        fieldUnit.position.Value = cell.gameObject.transform.position;
        fieldUnit.SummonUnit (card, player, cell);

        cell.FieldCard = fieldUnit;

        player.SummonUnit (fieldUnit);
        
        CallEffects ();
    }

    // void SwitchPriority () {
    //     if (!IsServer) return;

    //     //Here we swap the priority 
    //     if (playerTurn.Equals (player1)) {
    //         playerTurn = player2;
    //     } else {
    //         playerTurn = player1;
    //     }


    //     SwitchPriorityClientRpc (playerTurn.OwnerClientId);
    // }
    // [ClientRpc]
    // void SwitchPriorityClientRpc (ulong netid) {
    //     if (NetworkManager.Singleton.LocalClientId == netid)
    //         localHasPriority = true;
    //     else 
    //         localHasPriority = false;
    // }

    public void MoveUnit (FieldUnit unit, Vector2 cell, ulong netid) {
        if (playerTurn.OwnerClientId != netid) return;
        if (unit.OwnerClientId != netid) return;
        if (HexagonMetrics.GetDistantce (unit.Cell.Position, cell) > unit.movementSpeed.Value) return;
        if (unit.currActionPoints.Value <= 0) return;

        unit.Player.MoveUnit (unit, cell);
        unit.ConsumeActionPoint (1);
    }

    public void UnitAttack (FieldUnit attacker, Vector2 cell, ulong netid) {

        if (playerTurn.OwnerClientId != netid) return;
        if (attacker.OwnerClientId != netid) return;
        if (HexagonMetrics.GetDistantce (attacker.Cell.Position, cell) > attacker.attackRange.Value) return;
        if (attacker.currActionPoints.Value <= 0) return;

        HexagonCell hexCell;

        if (attacker.Player.MatchManage.FieldGrid.Cells.TryGetValue (cell, out hexCell)) {
            if (hexCell.FieldCard) {
                if (hexCell.FieldCard is FieldUnit)  {

                    FieldUnit target = hexCell.FieldCard as FieldUnit;

                    if (attacker.Player.FieldUnits.Contains (target)) return; //Return if target unit is a friendly.

                    Damage damage = new Damage (attacker.strength.Value, DamageSource.Attack, attacker.Player);
                    attacker.Player.DamageTarget (hexCell.FieldCard as FieldUnit, damage);

                    attacker.ConsumeActionPoint (1);
                }
            }
        }

    }

    public void PassTurn (Player player) {
        if (!IsServer) return;

        //If the player requesting the pass is the player who has the turn... 
        if (player.Equals(playerTurn)) {            
            if (turnNumber.Value % 2 == 0)
                playerTurn = player1;
            else
                playerTurn = player2;

            PassTurnClientRPC (playerTurn.OwnerClientId);

            EndTurn ();
        }
        
    }
    [ClientRpc]
    void PassTurnClientRPC (ulong netid) {
        if (NetworkManager.Singleton.LocalClientId == netid)
            localPlayerTurn = true;
        else 
            localPlayerTurn = false;
    }


    public void AddEffectToStack (CardEffect effect, FieldCard fieldCard) {
        CardEffect _effect = Instantiate<CardEffect> (effect);
        _effect.FieldCard = fieldCard;

        effectStack.Push (_effect);
    }

    public void CallEffects () {
        if (effectStack.Count > 0) {
            CardEffect effect = effectStack.Pop();
            if (effect.FieldCard != null) {
                effect.DoEffect ();
            }
            CallEffects ();
        }
    }

    //End Turn here!
    void EndTurn () {
        if (!IsServer) return;

        //TODO end turn stuff here
        playerTurn.EndTurn (turnNumber.Value);

        StartTurn ();
    }

    void StartTurn () {
        if (!IsServer) return;

        turnNumber.Value++;

        playerTurn.StartTurn (turnNumber.Value);
    }

    public bool LocalPlayerTurn {
        get {
            return localPlayerTurn;
        }
    }

    public HexagonGrid FieldGrid {get {return fieldGrid;}}
   
    public List<FieldUnit> AllUnits {
        get {
            List<FieldUnit> allUnits = new List<FieldUnit> ();

            foreach (FieldUnit unit in player1.FieldUnits) {
                allUnits.Add (unit);
            }

            foreach (FieldUnit unit in player2.FieldUnits) {
                allUnits.Add (unit);
            }

            return allUnits;
        }
    }

}
