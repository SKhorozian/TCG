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

    //Events
    #region  Events and Delegates
    public delegate void OnSummonFieldCardDelegate (FieldCard summonedCard);    //Delegate for when a card is summoned.
    public event OnSummonFieldCardDelegate fieldCardSummonedEvent;                   

    public delegate void OnDamageDelegate (Damage damage, IDamageable target);  //Delegate for when damage is dealt.
    public event OnDamageDelegate preDamageEvent;   //Before the damage is dealt.
    public event OnDamageDelegate postDamageEvent;  //After the damage is dealt.
        
    public delegate void OnHealDelegate (Heal heal, IDamageable target);  //Delegate for when healing is done.
    public event OnHealDelegate preHealEvent;   //Before healing.
    public event OnHealDelegate postHealEvent;  //After healing.

    public delegate void OnUnitStrike (FieldUnit striker, IDamageable target, Damage damage); //Delegate for when a unit strikes.
    public event OnUnitStrike preStrikeEvent;   //Before a unit strikes.
    public event OnUnitStrike postStrikeEvent;  //After a unit strikes.

    public delegate void OnFieldCardAct (FieldCard actor, ActionAbility action);    //Delegate for when a field card acts.
    public event OnFieldCardAct fieldCardActEvent;

    public delegate void OnFieldCardDestroy (FieldCard destroyed);      //Delegate for when a field card is destroyed (killed or demolished).
    public event OnFieldCardDestroy fieldCardDestroyEvent;

    public delegate void OnFieldCardBanish (FieldCard banished);    //Delegate for when a field card is banished.
    public event OnFieldCardBanish fieldCardBanishEvent;

    public delegate void OnCardBanish (CardInstance cardInstance);    //Delegate for when a card is banished.
    public event OnCardBanish cardBanishEvent;

    public delegate void OnPlayCard (CardInstance playedCard);  //Delegate for when a card is played.
    public event OnPlayCard playCardEvent;
    
    public delegate void OnCardCreate (CardInstance createdCard);   //Delegate for when a card is created.
    public event OnCardCreate createCardEvent;

    public delegate void OnDiscard (CardInstance discardedCard);    //Delegate for when a card is discarded.
    public event OnDiscard discardEvent;

    public delegate void OnSacrifice (FieldCard sacrifice);     //Delegate for when a field card is sacrificed.
    public event OnSacrifice sacrificeEvent;

    public delegate void OnManaSpend (int amount);  //Delegate for when mana is spent.
    public event OnManaSpend manaSpendEvent;

    public delegate void OnSpellResolve (SpellCardInstance spell);  //Delegate for when a spell resolves.
    public event OnSpellResolve spellResolveEvent;

    public delegate void OnActionResolve (ActionAbility action); //Delegate for when an action resolves.
    public event OnActionResolve actionResolveEvent;

    public delegate void OnPlayAbilityResolve (PlayAbility playAbility); //Delegate for when a play ability resolves.
    public event OnPlayAbilityResolve playAbilityResolve; 

    #endregion

    #region Gameplay

    //Starts the game
    public void StartGame () {
        if (!IsServer || gameStart) return;

    
        //Initialize Player stats
        maxMana.Value = 0;
        currentMana.Value = 0;

        // We tell all the clients that the game has started
        StartGameClientRPC();

        if (IsOwner) {
            matchManager = Instantiate(matchManagerPrefab).GetComponent<MatchManager> ();
            matchManager.gameObject.GetComponent<NetworkObject>().Spawn();
            matchManager.InitializeMatch();
        }
        gameStart = true;

        InitializeDeck();
        Draw(4); //TODO offer mulligan instead
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
            else 
                playCardEvent?.Invoke (card);

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
    public List<CardInstance> Draw (int number) {
        if (!IsServer) return null;

        List<CardInstance> drawnCards = new List<CardInstance> ();

        //We Loop for the number of times we want to draw.
        for (int i = 0; i < number; i++) {
            if (playerDeck.CurrentDeckSize <= 0 ) {
                //No more cards in the deck
                //Do something
                break;
            }

            CardInstance drawnCard = playerDeck.Draw();

            drawnCards.Add (drawnCard);
            AddToHand (drawnCard);
        }

        Debug.Log("Player: " + OwnerClientId + " drew " + number + " cards.");

        UpdatePlayerHand ();

        return drawnCards;
    }

    public CardInstance DrawAt (int i) {
        if (!IsServer) return null;

        if (i >= CurrentDeck.Deck.Count) return null;
        if (i < 0) return null;

        CardInstance drawnCard = playerDeck.Draw (i);
        AddToHand (drawnCard);

        Debug.Log("Player: " + OwnerClientId + " drew " + drawnCard.CardName + ".");

        UpdatePlayerHand ();

        return drawnCard;
    }

    public void AddToHand (CardInstance cardInstance) {
        if (!IsServer) return;

        if (playerHand.Count >= 10) {
            //Player hand limited reached
            //Do something
            DiscardCard (cardInstance);
            return;
        }

        playerHand.Add(cardInstance);
        UpdatePlayerHand();
    }

    void UpdatePlayerHand () {
        if (!IsServer) return;

        string[] cardLocations = new string [playerHand.Count];
        CardInstanceInfo[] cardInfos = new CardInstanceInfo[playerHand.Count];

        for (int i = 0; i < playerHand.Count; i++) {
            cardLocations[i] = playerHand[i].CardLocation;

            if (playerHand[i] is UnitCardInstance) {
                UnitCardInstance unitCardInstance = playerHand[i] as UnitCardInstance;

                cardInfos[i] = new CardInstanceInfo (playerHand[i].CostChange, unitCardInstance.PowerBonus, unitCardInstance.HealthBonus, unitCardInstance.RangeBonus, unitCardInstance.SpeedBonus, new StaticKeyword[0]);
            }
        }

        UpdatePlayerHandClientRPC (cardLocations, cardInfos);
    }

    public CardInstance DiscardCard (int n) {
        if (!IsServer) return null;
        if (playerHand.Count > n) {
            CardInstance card = playerHand[n];
            playerHand.RemoveAt (n);

            DiscardCard (card);

            return card;
        }

        UpdatePlayerHand ();

        return null;
    }

    public void DiscardCard (CardInstance card) {
        if (!IsServer) return;
        junkyard.Add (card);

        discardEvent?.Invoke (card);
    }

    public CardInstance CreateCard (Card card) {
        CardInstance createdCard;

        //Create a card that suits its type
        switch (card.Type) {
            case CardType.Unit:
                if (!(card is UnitCard)) throw new System.Exception ("Incorrect Card Type");
                createdCard = new UnitCardInstance (card);
                break;
            case CardType.Spell:
                if (!(card is SpellCard)) throw new System.Exception ("Incorrect Card Type");
                createdCard = new SpellCardInstance (card);
                break;
            case CardType.Structure:
                if (!(card is StructureCard)) throw new System.Exception ("Incorrect Card Type");
                createdCard = new StructureCardInstance (card);
                break;
            case CardType.Hero:
                if (!(card is HeroCard)) throw new System.Exception ("Incorrect Card Type");
                createdCard = new HeroCardInstance (card);
                break;
            default:
                throw new System.Exception ("Card Type not found");
        }

        createdCard.MakeCreated ();
        createCardEvent?.Invoke (createdCard);

        return createdCard;
    }

    [ClientRpc]
    void UpdatePlayerHandClientRPC (string[] cardLocations, CardInstanceInfo[] cardInfos) {
        if (!IsOwner) return;

        CardInstance[] instances = new CardInstance [cardLocations.Length];

        for (int i = 0; i < instances.Length; i++) {
            
            Card c = Resources.Load<Card> (cardLocations[i]);

            switch (c.Type) {
                case CardType.Unit: {
                    instances[i] = new UnitCardInstance (Resources.Load<Card> (cardLocations[i]), cardInfos[i]);
                } break;
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

    //For when a spell is successfully cast.
    public void SpellResolved (SpellCardInstance spellCard) {
        if (!IsServer) return;

        junkyard.Add (spellCard);

        spellResolveEvent?.Invoke (spellCard);
    }

    public void InitializeDeck () {
        if (NetworkManager.Singleton.IsServer) { 
            playerDeck = startDeck.InitializeDeck();
        }
    }

    public void ShuffleDeck () {
        if (!IsServer) return;
        
        playerDeck.ShuffleDeck();
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

        //Discard brief cards
        List<int> briefsToDiscard = new List<int> ();
        playerHand.ForEach (card => {
            if (card.IsBrief)
                briefsToDiscard.Add (playerHand.IndexOf (card));
        });
        foreach (int i in briefsToDiscard) {
            DiscardCard (i);
        }

        playerHero.TurnEnd ();

    }

    public void StartTurn (int turnNumber) {
        if (!IsServer) return;

        Draw (1);

        RampMana (1, true);
        currentMana.Value = maxMana.Value;

        foreach (FieldUnit unit in controledUnits) {
            unit.TurnStart ();
        }

        foreach (FieldStructure structure in controledStructures) {
            structure.TurnStart ();
        }

        playerHero.TurnStart ();
    }


    public void ButtonPress (){
        if (IsServer) {
            matchManager.ButtonPress (this);
        } else {
            ButtonPressRequestServerRpc ();
        }
    }

    [ServerRpc]
    void ButtonPressRequestServerRpc () {
        ButtonPress();
    }

    
    public Damage DamageTarget (IDamageable target, Damage damage) { //Deal damage to target.
        preDamageEvent?.Invoke (damage, target);
        
        target.TakeDamage (damage);

        postDamageEvent?.Invoke (damage, target);
        return damage;
    }

    public Heal HealTarget (IDamageable target, Heal heal, CardInstance source) {
        target.TakeHeal (heal);
        return heal;
    }

    public Heal HealTarget (IDamageable target, Heal heal) {
        target.TakeHeal (heal);
        return heal;
    }

    #endregion

    #region Unit 

    public void SummonUnit (FieldUnit unit) {
        if (!IsServer) return;

        fieldCardSummonedEvent?.Invoke (unit);

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

        fieldCardDestroyEvent?.Invoke (unit);

        //Trigger Scavenger
        ScavengerStatic.Trigger (matchManager);

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

        fieldCardDestroyEvent?.Invoke (structure);

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


    #region Special Keywords

    public void Crystalize (int rampAmount) {
        if (!IsServer) return;
        RampMana (rampAmount, false);
    }

    public void Sacrifice (FieldUnit unit) {
        if (!IsServer) return;
        UnitToDie (unit);
        sacrificeEvent?.Invoke (unit);
    }

    public void Sacrifice (FieldStructure structure) {
        if (!IsServer) return;
        StructureToDemolish (structure);
        sacrificeEvent?.Invoke (structure);
    }

    #endregion

    //Player Stats

    public void SpendMana (int cost) {
        if (!IsServer) return;
        currentMana.Value -= cost;

        currentMana.Value = Mathf.Clamp (currentMana.Value, 0, maxMana.Value);
    
        manaSpendEvent?.Invoke (cost);
    
        matchManager.eventTracker.AddEvent (new SpendManaEvent (this, matchManager.TurnNumber, cost));

        UpdatePlayerStatsClientRPC ();
    }

    //UI
    [ClientRpc]
    public void UpdateStackClientRPC (string[] targetorLocations, ulong[] playerIDs) {
        if (!IsLocalPlayer) return;

        Targetor[] targetors = new Targetor[targetorLocations.Length];

        if (targetorLocations.Length != playerIDs.Length) return; //If the array lengths don't match, return.

        for (int i = 0 ; i < targetors.Length; i++) {
            Targetor targetor = Resources.Load<Targetor> (targetorLocations[i]); //Find the targetor
            targetors[i] = Instantiate<Targetor> (targetor); //Create a new targetor
            targetors[i].name = targetor.name;

            if (playerIDs[i] == matchManager.Player1.OwnerClientId)
                targetors[i].Player = matchManager.Player1;
            else
                targetors[i].Player = matchManager.Player2;
            
            // List<ITargetable> newTargets = Targeting.ConvertTargets (targetors[i].TargetTypes, targets[i], targetors[i].Player);

            // if (targetors[i].TragetVaildity (newTargets))
            //     targetors[i].SetTargets (newTargets);
        }

        playerController.StackUI.UpdateStack (targetors);
    }

    public void RefillMana (int refillAmount) {
        if (!IsServer) return;
        currentMana.Value += refillAmount;

        currentMana.Value = Mathf.Clamp (currentMana.Value, 0, 10);
        UpdatePlayerStatsClientRPC ();
    }

    public void RampMana (int rampAmount, bool fillsMana) {
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
    
    public List<CardInstance> Deck                  {get {return CurrentDeck.Deck;}}
    public List<CardInstance> Graveyard             {get {return graveyard;}}
    public List<CardInstance> Junkyard              {get {return junkyard;}}
    public List<CardInstance> Banishyard            {get {return banishyard;}}
}
