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

    [SerializeField] FieldHero playerHero;

    //All used spells and dead units go here
    [SerializeField] private List<CardInstance> graveyard = new List<CardInstance> ();  //This is for dead units and demolished structures.
    [SerializeField] private List<CardInstance> junkyard = new List<CardInstance> ();   //This is for used spells and discarded cards.
    [SerializeField] private List<CardInstance> banishyard = new List<CardInstance> (); //This is for banished cards.

    //Controled Field Cards
    [SerializeField] List<FieldUnit> controledUnits;            //All units under this player's control.
    [SerializeField] List<FieldStructure> controledStructures;  //All structures under this player's control.

    //UI
    [SerializeField] GameObject playerControllerPrefab;
    PlayerController playerController;

    [SerializeField] GameObject playerStatsPrefab;
    PlayerStatsDisplay playerStats;

    [SerializeField] GameObject matchManagerPrefab;
    [SerializeField] MatchManager matchManager;

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

            playerStats.GetComponent<RectTransform> ().anchoredPosition = new Vector3 (-200, -1000, 0);
        } 

        playerStats.UpdateDisplay(currentMana.Value, maxMana.Value);

    }

    public void PlayCard (int c, Vector2 placement, Vector2[] targets, Vector2[] extraCostTargets) {
        if (IsServer) {
            CardInstance card = playerHand[c];
            playerHand.RemoveAt(c);

            if (!matchManager.PlayCard (card, this, placement, targets, extraCostTargets))
                AddToHand (card);

            UpdatePlayerHand();
        } else {
            PlayCardRequestServerRPC(c, placement, targets, extraCostTargets);
        }
    }

    [ServerRpc]
    void PlayCardRequestServerRPC (int c, Vector2 placement, Vector2[] targets, Vector2[] extraCostTargets) {
        PlayCard(c, placement, targets, extraCostTargets);
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

    public void AddToHand (CardInstance cardInstance) {
        if (!IsServer) return;

        if (playerHand.Count >= 10) {
            //Player hand limited reached
            //Do something
            return;
        }

        playerHand.Add(cardInstance);
        UpdatePlayerHand();
    }

    void UpdatePlayerHand () {
        if (!IsServer) return;

        string[] cardLocations = new string [playerHand.Count];

        for (int i = 0; i < playerHand.Count; i++) {
            cardLocations[i] = playerHand[i].CardLocation;
        }

        UpdatePlayerHandClientRPC (cardLocations);
    }

    public CardInstance DiscardCard (int n) {
        if (playerHand.Count > n) {
            CardInstance card = playerHand[n];
            playerHand.RemoveAt (n);

            DiscardCard (card);

            return card;
        }
        return null;
    }

    public void DiscardCard (CardInstance card) {
        //Call Discard effects

        junkyard.Add (card);
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
                case CardType.Structure:
                    instances[i] = new StructureCardInstance (Resources.Load<Card> (cardLocations[i]));
                    break;
                case CardType.Spell:
                    instances[i] = new SpellCardInstance (Resources.Load<Card> (cardLocations[i]));
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

    [ClientRpc]
    void UpdatePlayerStatsClientRPC () {
        if (IsClient && playerStats) 
            playerStats.UpdateDisplay(currentMana.Value, maxMana.Value);
    }

    void Update () {
        if (IsClient && playerStats) 
            playerStats.UpdateDisplay(currentMana.Value, maxMana.Value);
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

    public void EndTurn (int turnNumber) {

        foreach (FieldUnit unit in controledUnits) {
            unit.TurnEnd ();
        }

        foreach (FieldStructure structure in controledStructures) {
            structure.TurnEnd ();
        }

    }

    public void StartTurn (int turnNumber) {
        if (!IsServer) return;

        Draw (1);

        RampManaPermanent (1, true);
        currentMana.Value = maxMana.Value;

        foreach (FieldUnit unit in controledUnits) {
            unit.TurnStart ();
        }

        foreach (FieldStructure structure in controledStructures) {
            structure.TurnStart ();
        }

        matchManager.CallEffects ();

    }


    public void PassTurn (){
        if (IsServer) {
            matchManager.PassTurn (this);
        } else {
            RequestPassTurnServerRpc ();
        }
    }

    [ServerRpc]
    void RequestPassTurnServerRpc () {
        PassTurn();
    }

    
    public Damage DamageTarget (IDamageable target, Damage damage, FieldCard source) { //Deal damage to target.
        target.TakeDamage (damage);
        return damage;
    }

    public Damage DamageTarget (IDamageable target, Damage damage, SpellCardInstance source) { //Deal damage to target.
        target.TakeDamage (damage);
        return damage;
    }

    public Heal HealTarget (IDamageable target, Heal heal, FieldCard source) {
        target.TakeHeal (heal);
        return heal;
    }

    public Heal HealTarget (IDamageable target, Heal heal, SpellCardInstance source) {
        target.TakeHeal (heal);
        return heal;
    }

    #endregion

    #region Unit 

    public void SummonUnit (FieldUnit unit) {
        if (!IsServer) return;

        controledUnits.Add (unit);
    }

    public void MoveUnit (FieldUnit unit, HexagonCell cell) {
        if (!IsServer) return;

        if (cell.FieldCard != null) return;

        unit.Cell.FieldCard = null;

        cell.FieldCard = unit;
        unit.Cell = cell;

        unit.position.Value = cell.transform.position;

        unit.UpdateUnit ();

    }

    //Add death to the stack
    public void UnitToDie (FieldUnit unit) {
        if (!IsServer) return;
        if (!controledUnits.Contains (unit)) return;

        matchManager.AddEffectToStack(new UnitDeath (unit));
    }

    //Final Death effect
    public void UnitDie (FieldUnit unit) {
        if (!IsServer) return;
        if (!controledUnits.Contains (unit)) return;

        graveyard.Add (unit.UnitsCard);

        FieldUnits.Remove (unit);

        unit.NetworkObject.Despawn (true);
        unit.Cell.FieldCard = null;

    }

    #endregion 

    #region Structure

    public void SummonStructure (FieldStructure structure) {
        if (!IsServer) return;

        controledStructures.Add (structure);
    }

    //Add demolition to the stack
    public void StructureToDemolish (FieldStructure structure) {
        if (!IsServer) return;
        if (!controledStructures.Contains (structure)) return;

        matchManager.AddEffectToStack (new StructureDemolition (structure));
    }

    public void StructureDemolish (FieldStructure structure) {
        if (!IsServer) return;
        if (!controledStructures.Contains (structure)) return;

        graveyard.Add (structure.StructursCard);

        FieldStructures.Remove (structure);

        structure.NetworkObject.Despawn (true);
        structure.Cell.FieldCard = null;
    }

    #endregion

    #region Hero

    public void SummonHero (FieldHero fieldHero) {
        if (!IsServer) return;
        playerHero = fieldHero;
    }


    #endregion


    //Player Stats

    public void SpendMana (int cost) {
        if (!IsServer) return;
        currentMana.Value -= cost;

        currentMana.Value = Mathf.Clamp (currentMana.Value, 0, maxMana.Value);
        UpdatePlayerStatsClientRPC ();
    }

    public void RefillMana (int refillAmount) {
        if (!IsServer) return;
        currentMana.Value += refillAmount;

        currentMana.Value = Mathf.Clamp (currentMana.Value, 0, maxMana.Value);
        UpdatePlayerStatsClientRPC ();
    }

    public void RampManaPermanent (int rampAmount, bool fillsMana) {
        if (!IsServer) return;
        maxMana.Value += rampAmount;
        maxMana.Value = Mathf.Clamp (maxMana.Value, 0, 10);
        if (fillsMana) {
            RefillMana (rampAmount);
        }
        UpdatePlayerStatsClientRPC ();
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

    public GameDeck CurrentDeck                     {get {return playerDeck;}}
    public Deck StartDeck                           {get {return startDeck;}}

    public List<FieldUnit> FieldUnits               {get {return controledUnits;}}
    public List<FieldStructure> FieldStructures     {get {return controledStructures;}}
    public FieldHero FieldHero                      {get {return playerHero;}}
    

}
