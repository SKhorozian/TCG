using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;
using MLAPI.Messaging;

public class Player : NetworkBehaviour 
{
    bool gameStart = false;
    public List<Card> playerHand = new List<Card> ();

    [SerializeField] private List<Card> playerDeck = new List<Card>();

    //UI
    [SerializeField] GameObject playerHandPanelPrefab;
    PlayerHandDisplay playerHandDisplay;

    [SerializeField] GameObject playerStatsPrefab;
    PlayerStatsDisplay playerStats;

    [SerializeField] GameObject matchManagerPrefab;
    [SerializeField] MatchManager matchManager;

    [SerializeField] GameObject turnButtonPrefab;

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
            playerHandDisplay = Instantiate(playerHandPanelPrefab, canvas.transform).GetComponent<PlayerHandDisplay>();

            playerStats.GetComponent<RectTransform> ().anchoredPosition = new Vector3 (-200, -950, 0);

            Instantiate (turnButtonPrefab, canvas.transform);
        } 

        playerStats.UpdateDisplay(heroHealth.Value, currentMana.Value, maxMana.Value);

    }

    public void PlayCard (int c) {
        if (IsServer) {
            if (matchManager.PlayCard (playerHand[c], this)) {
                SpendMana (playerHand[c].Cost);
                playerHand.RemoveAt(c);
            }
            UpdatePlayerHand();
        } else {
            PlayCardRequestServerRPC(c);
        }
    }

    [ServerRpc]
    void PlayCardRequestServerRPC (int c) {
        PlayCard(c);
    }


    //Draws a number of cards
    public void Draw (int number) {
        if (NetworkManager.Singleton.IsServer) {

            //We Loop for the number of times we want to draw.
            for (int i = 0; i < number; i++) {
                if (playerDeck.Count <= 0 ) {
                    //No more cards in the deck
                    //Do something
                    break;
                }

                if (playerHand.Count >= 10) {
                    //Player hand limited reached
                    //Do something
                    break;
                }

                playerHand.Add(playerDeck[playerDeck.Count-1]);
                playerDeck.RemoveAt(playerDeck.Count-1);
            }

            Debug.Log("Player: " + NetworkManager.ServerClientId + " drew " + number + " cards.");

            UpdatePlayerHand ();

        } else {
            //We call an RPC
            DrawServerRpc(number);
            Debug.Log("Player: " + NetworkManager.ServerClientId + " sent an RPC to draw " + number + " cards.");
        }
    }

    void UpdatePlayerHand () {
        if (!IsServer) return;

        string[] cardLocations = new string [playerHand.Count];

        for (int i = 0; i < playerHand.Count; i++) {
            cardLocations[i] = "Cards/" + playerHand[i].Color.ToString() + "/" + playerHand[i].CardName;
        }
        
        UpdatePlayerHandClientRPC (cardLocations);
    }

    [ClientRpc]
    void UpdatePlayerHandClientRPC (string[] cardLocations) {
        if (!IsOwner) return;

        Card[] cards = new Card[cardLocations.Length];

        for (int i = 0; i < cards.Length; i++) {
            cards[i] = Resources.Load<Card> (cardLocations[i]);
        }

        playerHandDisplay.UpdateCardDisplays (cards);

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
            List<Card> deck = new List<Card>();

            foreach (Card c in playerDeck) {
                deck.Add(Object.Instantiate (c));
            }

            playerDeck = deck;
            ShuffleDeck();
        }
    }

    public void ShuffleDeck () {
        if (NetworkManager.Singleton.IsServer) { 
            List<Card> cards = new List<Card> ();

            int deckSize = playerDeck.Count;
            

            for (int i = 0; i < deckSize; i++) {
                int r = Random.Range (0, playerDeck.Count);

                cards.Add(playerDeck[r]);
                playerDeck.RemoveAt(r);
            }

            playerDeck = cards;
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

        if (playerHandDisplay)
            DestroyImmediate (playerHandDisplay.gameObject);

        if (playerStats)
            DestroyImmediate (playerStats.gameObject);
    }

    [ServerRpc]
    void EndGameRequestServerRpc () {
        
    }

    public void StartTurn (int turnNumber) {
        if (!IsServer) return;

        Draw (1);

        maxMana.Value++;
        maxMana.Value = Mathf.Clamp (maxMana.Value, 0, 10);

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

    //Player Stats

    public void SpendMana (int cost) {
        if (!IsServer) return;
        currentMana.Value -= cost;

        currentMana.Value = Mathf.Clamp (currentMana.Value, 0, maxMana.Value);
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

}
