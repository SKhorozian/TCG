using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;
using MLAPI.Messaging;

public class Player : NetworkBehaviour 
{
    bool gameStart = false;
    public List<CardInstance> playerHand = new List<CardInstance> ();

    [SerializeField] private Deck startDeck;
    [SerializeField] private GameDeck playerDeck;

    //All used spells and dead units go here
    [SerializeField] private List<CardInstance> graveyard;
    [SerializeField] private List<CardInstance> destroyedCards;

    //Controled Field Cards
    [SerializeField] List<FieldUnit> controledUnits;

    //UI
    [SerializeField] GameObject playerControllerPrefab;
    PlayerController playerController;

    [SerializeField] GameObject playerStatsPrefab;
    PlayerStatsDisplay playerStats;

    [SerializeField] GameObject matchManagerPrefab;
    [SerializeField] MatchManager matchManager;



    //Stats:
    //Health
    NetworkVariableInt heroHealth = new NetworkVariableInt(new NetworkVariableSettings{
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.ServerOnly
    });

    //Mana
    NetworkVariableInt maxMana = new NetworkVariableInt(new NetworkVariableSettings{
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.ServerOnly
    });
    NetworkVariableInt currentMana = new NetworkVariableInt(new NetworkVariableSettings{
        ReadPermission = NetworkVariablePermission.Everyone,
        WritePermission = NetworkVariablePermission.ServerOnly
    });

    #region Gameplay

    //Starts the game
    public void StartGame () {
        if (!IsServer || gameStart) return;

    
        //Initialize Player stats
        heroHealth.Value = 3000;
        maxMana.Value = 1;
        currentMana.Value = 1;

        // We tell all the clients that the game has started
        StartGameClientRPC();

        if (IsOwner) {
            matchManager = Instantiate(matchManagerPrefab).GetComponent<MatchManager> ();
            matchManager.gameObject.GetComponent<NetworkObject>().Spawn();
            matchManager.InitializeMatch();
        }
        gameStart = true;

        InitializeDeck();
        Draw(5); //TODO offer mulligan instead
    }

    [ClientRpc]
    void StartGameClientRPC () {

        GameObject canvas = GameObject.FindObjectOfType<Canvas>().gameObject;
        playerStats = Instantiate(playerStatsPrefab, canvas.transform).GetComponent<PlayerStatsDisplay> ();

        if (IsOwner)
        {
            playerController = Instantiate(playerControllerPrefab, canvas.transform).GetComponent<PlayerController> ();

            playerStats.GetComponent<RectTransform> ().anchoredPosition = new Vector3 (-200, -950, 0);
        } 

        playerStats.UpdateDisplay(heroHealth.Value, currentMana.Value, maxMana.Value);

    }

    public void PlayCard (int c, Vector2 cell) {
        if (IsServer) {
            if (matchManager.PlayCard (playerHand[c], this, cell)) {
                playerHand.RemoveAt(c);
            }
            UpdatePlayerHand();
        } else {
            PlayCardRequestServerRPC(c, cell);
        }
    }

    [ServerRpc]
    void PlayCardRequestServerRPC (int c, Vector2 cell) {
        PlayCard(c, cell);
    }


    //Draws a number of cards
    public void Draw (int number) {
        if (NetworkManager.Singleton.IsServer) {

            //We Loop for the number of times we want to draw.
            for (int i = 0; i < number; i++) {
                if (playerDeck.CurrentDeckSize <= 0 ) {
                    //No more cards in the deck
                    //Do something
                    break;
                }

                if (playerHand.Count >= 10) {
                    //Player hand limited reached
                    //Do something
                    break;
                }

                playerHand.Add(playerDeck.Draw());
            }

            Debug.Log("Player: " + OwnerClientId + " drew " + number + " cards.");

            UpdatePlayerHand ();

        } else {
            //We call an RPC
            DrawServerRpc(number);
            Debug.Log("Player: " + OwnerClientId + " sent an RPC to draw " + number + " cards.");
        }
    }

    void UpdatePlayerHand () {
        if (!IsServer) return;

        string[] cardLocations = new string [playerHand.Count];

        for (int i = 0; i < playerHand.Count; i++) {
            cardLocations[i] = playerHand[i].CardLocation;
        }

        UpdatePlayerHandClientRPC (cardLocations);
    }

    [ClientRpc]
    void UpdatePlayerHandClientRPC (string[] cardLocations) {
        if (!IsOwner) return;

        CardInstance[] instances = new CardInstance [cardLocations.Length];

        for (int i = 0; i < instances.Length; i++) {
            
            Card c = Resources.Load<Card> (cardLocations[i]);

            switch (c.Type) {
                case CardType.Unit:
                    instances[i] = new UnitCardInstance (Resources.Load<Card> (cardLocations[i]));
                    break;
            }

            
        }

        playerController.UpdateCardDisplays (instances);
    }

    [ServerRpc]
    void DrawServerRpc(int number) {
        if (!IsServer) return;

        Draw (number);
    }

    //We draw a specific card
    public void Draw (Card card) {
        if (NetworkManager.Singleton.IsServer) {
            
        } else {
            //We call an RPC
        }
    }       

    public void InitializeDeck () {
        if (NetworkManager.Singleton.IsServer) { 
            playerDeck = startDeck.InitializeDeck();
        }
    }

    public void ShuffleDeck () {
        if (NetworkManager.Singleton.IsServer) { 
            playerDeck.ShuffleDeck();
        } else {
            ShuffleDeckServerRpc ();
        } 
    }

    [ServerRpc]
    void ShuffleDeckServerRpc () {
        if (!IsServer) return;

        ShuffleDeck();
    }

    void UpdatePlayerStats () {
        playerStats.UpdateDisplay(heroHealth.Value, currentMana.Value, maxMana.Value);
    }

    void Update () {
        if (IsClient && playerStats) 
            playerStats.UpdateDisplay(heroHealth.Value, currentMana.Value, maxMana.Value);
    }

    void OnDestroy () {
        EndGame();
    }

    public void EndGame () {
        if (!IsOwner) return;

        if (playerController)
            DestroyImmediate (playerController.gameObject);

        if (playerStats)
            DestroyImmediate (playerStats.gameObject);
    }

    [ServerRpc]
    void EndGameRequestServerRpc () {
        
    }

    public void StartTurn (int turnNumber) {
        if (!IsServer) return;

        Draw (1);

        RampManaPermanent (1, true);
        currentMana.Value = maxMana.Value;

    }


    public void PassAction (){
        if (IsServer) {
            matchManager.PassAction (this);
        } else {
            RequestPassActionServerRpc ();
        }
    }

    [ServerRpc]
    void RequestPassActionServerRpc () {
        PassAction();
    }

    
    public Damage DamageTarget (Player player, Damage damage) { //Deal damage to target player.
        return damage;
    }

    public Damage DamageTarget (FieldUnit unit, Damage damage) { //Deal damage to target unit.
        return damage;
    }

    #endregion

    #region Unit Stuff 

    public void SummonUnit (FieldUnit unit) {
        if (!IsServer) return;

        controledUnits.Add (unit);
    }

    public void MoveUnit (FieldUnit unit, Vector2 cell) {
        if (!IsServer) return;

        //Check if this unit can move
        if (unit.actionPoints.Value <= 0) return;

        HexagonCell hexCell;

        if (matchManager.FieldGrid.Cells.TryGetValue (cell, out hexCell)) {
            if (hexCell.FieldCard != null) return;

            unit.Cell.FieldCard = null;

            hexCell.FieldCard = unit;
            unit.Cell = hexCell;

            unit.position.Value = hexCell.transform.position;

            unit.UpdateUnit ();
        } 

    }

    public void UnitDie (FieldUnit unit) {
        if (!IsServer) return;

        FieldUnits.Remove (unit);
    }

    #endregion 


    //Player Stats

    public void SpendMana (int cost) {
        if (!IsServer) return;
        currentMana.Value -= cost;

        currentMana.Value = Mathf.Clamp (currentMana.Value, 0, maxMana.Value);
    }

    public void RefillMana (int refillAmount) {
        if (!IsServer) return;
        currentMana.Value += refillAmount;

        currentMana.Value = Mathf.Clamp (currentMana.Value, 0, maxMana.Value);
    }

    public void RampManaPermanent (int rampAmount, bool fillsMana) {
        if (!IsServer) return;
        maxMana.Value += rampAmount;
        maxMana.Value = Mathf.Clamp (maxMana.Value, 0, 10);
        if (fillsMana) {
            RefillMana (rampAmount);
        }
    }


    //Getters and setters
    public bool HasGameStart {
        get {return gameStart;}
    }

    public MatchManager MatchManage {
        get {
            return matchManager;
        }
        set {
            matchManager = value;
        }
    }

    public int CurrentMana {
        get {
            return currentMana.Value;
        }
    }

    public int MaxMana {
        get {
            return maxMana.Value;
        }
    }

    public List<FieldUnit> FieldUnits {get {return controledUnits;}}

}
