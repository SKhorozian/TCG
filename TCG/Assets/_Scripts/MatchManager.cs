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

    [SerializeField] Player priority; //Who's turn is it to play.
    [SerializeField] bool localHasPriority; //Does the local client have the priority

    [SerializeField] GameObject fieldGridPrefab;
    HexagonGrid fieldGrid;

    [SerializeField] NetworkVariableBool lastActionWasPass = new NetworkVariableBool (new NetworkVariableSettings {
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.ServerOnly
    });


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

        priority = player1;
        localHasPriority = true;
    }

    //Returns true if the card was played successfully, else return false
    public bool PlayCard (Card card, Player player) {
        if (!IsServer) return false;

        if (!player.Equals(priority)) return false;
        if (player.CurrentMana < card.Cost) return false;

        //TODO the card actually plays

        SwitchPriority (); //It becomes the other player's turn to play

        lastActionWasPass.Value = false;

        return true;
    }

    void SwitchPriority () {
        if (!IsServer) return;

        //Here we swap the priority 
        if (priority.Equals (player1)) {
            priority = player2;
        } else {
            priority = player1;
        }


        SwitchPriorityClientRpc (priority.OwnerClientId);
    }

    [ClientRpc]
    void SwitchPriorityClientRpc (ulong netid) {
        if (NetworkManager.Singleton.LocalClientId == netid)
            localHasPriority = true;
        else 
            localHasPriority = false;
    }


    public void PassAction (Player player) {
        if (!IsServer) return;

        //If the player requesting the pass is the player who has the priority... 
        if (player.Equals(priority)) {
            if (lastActionWasPass.Value) {
                EndTurn ();
                lastActionWasPass.Value = false;
            } else {
                lastActionWasPass.Value = true;
            }

            SwitchPriority();
        }
        
    }

    //End Turn here!
    void EndTurn () {
        if (!IsServer) return;

        //TODO end turn stuff here

        StartTurn ();
    }

    void StartTurn () {
        if (!IsServer) return;

        turnNumber.Value++;

        player1.StartTurn (turnNumber.Value);
        player2.StartTurn (turnNumber.Value);

    }

    public bool LocalHasPriority {
        get {
            return localHasPriority;
        }
    }

}
