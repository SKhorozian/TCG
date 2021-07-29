using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;
using MLAPI.Spawning;

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
    [SerializeField] Player playerPriority; //Who's priority it is.
    [SerializeField] bool hasActed = false;

    [SerializeField] bool localPlayerTurn; //Does the local client have the priority
    [SerializeField] bool localPlayerPriority; //Does the local client have the priority

    [SerializeField] GameObject fieldGridPrefab;
    HexagonGrid fieldGrid;

    [SerializeField] GameObject fieldHeroPrefab;
    [SerializeField] GameObject fieldUnitPrefab;
    [SerializeField] GameObject fieldStructurePrefab;

    Stack<CardEffect> effectStack = new Stack<CardEffect> ();

    [SerializeField] List<Targetor> targetorStack = new List<Targetor> ();

    public GameEventTracker eventTracker;

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

        turnNumber.Value = 0;

        playerTurn = player1;
        playerPriority = player1;
        localPlayerTurn = true;
        localPlayerPriority = true;

        eventTracker = new GameEventTracker ();

        InitializeMatchClientRpc (player1.NetworkObjectId, player2.NetworkObjectId);
    }
    [ClientRpc]
    void InitializeMatchClientRpc (ulong player1ObjectId, ulong player2ObjectId) {

        if (NetworkSpawnManager.SpawnedObjects.TryGetValue (player1ObjectId, out var player1Object)) {
            Player player1 = player1Object.GetComponent<Player> ();
            player1.MatchManage = this;
            this.player1 = player1;
        }

        if (NetworkSpawnManager.SpawnedObjects.TryGetValue (player2ObjectId, out var player2Object)) {
            Player player2 = player2Object.GetComponent<Player> ();
            player2.MatchManage = this;
            this.player2 = player2;
        }

        fieldGrid = Instantiate (fieldGridPrefab).GetComponent<HexagonGrid> ();
        fieldGrid.InitializeGrid();

        if (IsServer) SummonHeros (); 
    }

    //Summon the heroes as Field Cards
    public void SummonHeros () {
        if (!IsServer) return;
        
        //Player 1:
        GameObject player1Hero = NetworkManager.Instantiate (fieldHeroPrefab, Vector3.zero, Quaternion.identity);
        player1Hero.gameObject.GetComponent<NetworkObject> ().SpawnWithOwnership (player1.OwnerClientId, null, true);

        HexagonCell cell1;

        if (fieldGrid.Cells.TryGetValue (new Vector2 (3, 13), out cell1)) {
            HeroCardInstance heroCardInstance = new HeroCardInstance (player1.StartDeck.HeroCard);
            FieldHero fieldHero = player1Hero.GetComponent<FieldHero> ();
            fieldHero.SummonHero (heroCardInstance, player1, cell1);

            fieldHero.position.Value = cell1.gameObject.transform.position;
            cell1.FieldCard = fieldHero;
            
            player1.SummonHero (fieldHero);
        } else {
            throw new System.Exception ("Invalid Hex Cell for player 1's hero!");
        }

        //Player 2:
        GameObject player2Hero = NetworkManager.Instantiate (fieldHeroPrefab, Vector3.zero, Quaternion.identity);
        player2Hero.gameObject.GetComponent<NetworkObject> ().SpawnWithOwnership (player2.OwnerClientId, null, true);

        HexagonCell cell2;

        if (fieldGrid.Cells.TryGetValue (new Vector2 (3, 1), out cell2)) {
            HeroCardInstance heroCardInstance = new HeroCardInstance (player2.StartDeck.HeroCard);
            FieldHero fieldHero = player2Hero.GetComponent<FieldHero> ();
            fieldHero.SummonHero (heroCardInstance, player2, cell2);

            fieldHero.position.Value = cell2.gameObject.transform.position;
            cell2.FieldCard = fieldHero;

            player2.SummonHero (fieldHero);
        } else {
            throw new System.Exception ("Invalid Hex Cell for player 2's hero!");
        }

        CallEffects ();

        StartTurn();
    }


    //Returns true if the card was played successfully, else return false
    public bool PlayCard (CardInstance card, Player player, Vector2 placement, Vector2[] targets, Vector2[] extraCostTargets) {
        if (!IsServer) return false;

        if (!player.Equals(playerPriority)) return false;
        if (player.CurrentMana < card.Cost) return false;

        switch (card.Type) { 
            case CardType.Unit: {
                if (!player.Equals(playerTurn)) return false;
                if (placement == null) return false;
                if (!(card is UnitCardInstance)) return false;

                HexagonCell cell;
                if (!fieldGrid.Cells.TryGetValue(placement, out cell)) {Debug.Log ("Placement invalid"); return false;};
                if (cell.FieldCard) return false; //If the cell is occupied return false

                //Spend Mana
                player.SpendMana (card.Cost);

                FieldUnit summonedUnit = SummonUnit (card, player, cell);

                //Extra Cost
                if (card.Card.ExtraCost) {
                    ExtraCost extraCost = Instantiate <ExtraCost> (card.Card.ExtraCost);
                    extraCost = card.Card.ExtraCost;
                    extraCost.Player = player;
                    
                    List<ITargetable> newTargets = Targeting.ConvertTargets (extraCost.TargetTypes, extraCostTargets, player);

                    if (extraCost.TragetVaildity(newTargets)) { //If targets are valid, then we proceed to play the card. Otherwise, we return.
                        extraCost.SetTargets (newTargets);
                        extraCost.DoEffect ();
                    } else {Debug.Log("Extra Cost failed targeting"); return false;};
                }

                //Play Effects
                if ((card as UnitCardInstance).UnitCard.OnPlayEffect) {
                    PlayAbility playEffect = Instantiate <PlayAbility> ((card as UnitCardInstance).UnitCard.OnPlayEffect);
                    playEffect.name = (card as UnitCardInstance).UnitCard.OnPlayEffect.name;
                    playEffect.FieldCard = summonedUnit;
                    playEffect.Player = player;
            
                    List<ITargetable> newTargets = Targeting.ConvertTargets (playEffect.TargetTypes, targets, player);

                    if (playEffect.TragetVaildity(newTargets) && summonedUnit) { //If targets are valid, then we proceed to call the play effect. Otherwise, we ignore it.
                        playEffect.SetTargets (newTargets);
                        AddTargetorToStack (playEffect);
                    } else {Debug.Log("Play Effect failed targeting");};
                }

                eventTracker.AddEvent (new PlayCardEvent (player, turnNumber.Value, card));
                CallEffects ();
                Debug.Log ("Player " + player.OwnerClientId + " played " + card.CardName + " on cell " + placement);
            } break;
            case CardType.Structure: {
                if (!player.Equals(playerTurn)) return false;
                if (placement == null) return false;
                if (!(card is StructureCardInstance)) return false;

                HexagonCell cell;
                if (!fieldGrid.Cells.TryGetValue(placement, out cell)) {Debug.Log ("Placement invalid"); return false;};
                if (cell.FieldCard) return false; //If the cell is occupied return false

                //Spend Mana
                player.SpendMana (card.Cost);

                FieldStructure summonedStructure = SummonStructure (card, player, cell);

                //Extra Cost
                if (card.Card.ExtraCost) {
                    ExtraCost extraCost = Instantiate <ExtraCost> (card.Card.ExtraCost);
                    extraCost = card.Card.ExtraCost;
                    extraCost.Player = player;

                    List<ITargetable> newTargets = Targeting.ConvertTargets (extraCost.TargetTypes, extraCostTargets, player);

                    if (extraCost.TragetVaildity(newTargets)) { //If targets are valid, then we proceed to play the card. Otherwise, we return.
                        extraCost.SetTargets (newTargets);
                        extraCost.DoEffect ();
                    } else {Debug.Log("Extra Cost failed targeting"); return false;};
                }

                //Play Effects
                if ((card as StructureCardInstance).StructureCard.OnPlayEffect) {
                    PlayAbility playEffect = Instantiate <PlayAbility> ((card as StructureCardInstance).StructureCard.OnPlayEffect);
                    playEffect.name = (card as StructureCardInstance).StructureCard.OnPlayEffect.name;
                    playEffect.FieldCard = summonedStructure;
                    playEffect.Player = player;

                    List<ITargetable> newTargets = Targeting.ConvertTargets (playEffect.TargetTypes, targets, player);

                    if (playEffect.TragetVaildity(newTargets) && summonedStructure) { //If targets are valid, then we proceed to call the play effect. Otherwise, we ignore it.
                        playEffect.SetTargets (newTargets);
                        AddTargetorToStack (playEffect);
                    } else {Debug.Log("Play Effect failed targeting");};
                }

                eventTracker.AddEvent (new PlayCardEvent (player, turnNumber.Value, card));
                CallEffects ();
                Debug.Log ("Player " + player.OwnerClientId + " played " + card.CardName + " on cell " + placement);
            } break;
            case CardType.Spell:
                if (!(card is SpellCardInstance)) return false;

                //Spell Effects
                if ((card as SpellCardInstance)._SpellCard.Spell) {
                    Spell spell = Instantiate <Spell> ((card as SpellCardInstance)._SpellCard.Spell);
                    spell.name = (card as SpellCardInstance)._SpellCard.Spell.name;
                    spell.SpellCard = card as SpellCardInstance;
                    spell.Player = player;
            
                    if (spell.Speed == TargetorPriority.Ritual && targetorStack.Count > 0) return false;

                    List<ITargetable> newTargets = Targeting.ConvertTargets (spell.TargetTypes, targets, player);

                    if (spell.TragetVaildity(newTargets)) { //If targets are valid, then we proceed to call the spell effect. Otherwise, the spell doesn't go off.

                        //Extra Cost
                        if (card.Card.ExtraCost) {
                            ExtraCost extraCost = Instantiate <ExtraCost> (card.Card.ExtraCost);
                            extraCost = card.Card.ExtraCost;
                            extraCost.Player = player;
                            
                            List<ITargetable> newExtraCostTargets = Targeting.ConvertTargets (extraCost.TargetTypes, extraCostTargets, player);

                            if (extraCost.TragetVaildity(newExtraCostTargets)) { //If targets are valid, then we proceed to play the card. Otherwise, we return.
                                extraCost.SetTargets (newExtraCostTargets);
                                extraCost.DoEffect ();
                            } else {Debug.Log("Extra Cost failed targeting"); return false;};
                        }   

                        spell.SetTargets (newTargets);
                        AddTargetorToStack (spell);
                    } else {Debug.Log("Spell failed targeting"); return false;};
                }

                //Spend Mana
                player.SpendMana (card.Cost);

                eventTracker.AddEvent (new PlayCardEvent (player, turnNumber.Value, card));
                CallEffects ();
                Debug.Log ("Player " + player.OwnerClientId + " casted " + card.CardName);
                break;
            default:
                break;
        }

        hasActed = true;

        return true;
    }

    FieldStructure SummonStructure (CardInstance card, Player player, HexagonCell cell) {
        if (!IsServer) return null;

        if (cell.FieldCard) return null; //If the cell is occupied return

        GameObject newStructure = Instantiate(fieldStructurePrefab, Vector3.zero, Quaternion.identity);
        newStructure.gameObject.GetComponent<NetworkObject> ().SpawnWithOwnership (player.OwnerClientId, null, true);

        FieldStructure fieldStructure = newStructure.GetComponent <FieldStructure> ();

        fieldStructure.position.Value = cell.gameObject.transform.position;
        fieldStructure.SummonStructure (card, player, cell);

        cell.FieldCard = fieldStructure;

        player.SummonStructure (fieldStructure);

        eventTracker.AddEvent (new SummonStructureEvent (player, turnNumber.Value, fieldStructure.StructursCard));

        return fieldStructure;        

    }

    FieldUnit SummonUnit (CardInstance card, Player player, HexagonCell cell) {
        if (!IsServer) return null;

        if (cell.FieldCard) return null; //If the cell is occupied return

        GameObject newUnit = Instantiate(fieldUnitPrefab, Vector3.zero, Quaternion.identity);
        newUnit.gameObject.GetComponent<NetworkObject>().SpawnWithOwnership ( player.OwnerClientId, null, true);

        FieldUnit fieldUnit = newUnit.GetComponent<FieldUnit> ();

        fieldUnit.position.Value = cell.gameObject.transform.position;
        fieldUnit.SummonUnit (card, player, cell);

        cell.FieldCard = fieldUnit;

        player.SummonUnit (fieldUnit);

        eventTracker.AddEvent (new SummonUnitEvent (player, turnNumber.Value, fieldUnit.UnitsCard));
        
        return fieldUnit;
    }

    public void MoveUnit (FieldUnit unit, Vector2[] targets, ulong netid) {
        if (!IsServer) return;

        if (playerTurn.OwnerClientId != netid) return;
        if (unit.OwnerClientId != netid) return;

        MovementAction moveAction = new MovementAction ();
        moveAction.FieldCard = unit;
        moveAction.Player = unit.Player;

        List <ITargetable> newTargets = Targeting.ConvertTargets (moveAction.TargetTypes, targets, unit.Player);

        if (moveAction.TragetVaildity (newTargets)) {
            moveAction.SetTargets (newTargets);
            moveAction.DoEffect ();
            CallEffects ();
        }
 
    }

    public void UnitAttack (FieldUnit attacker, Vector2[] targets, ulong netid) {
        if (!IsServer) return;

        if (playerTurn.OwnerClientId != netid) return;
        if (attacker.OwnerClientId != netid) return;

        if (attacker.currActionPoints.Value == 0) return; else attacker.ConsumeActionPoint ();

        AttackAction attackAction = new AttackAction ();
        attackAction.name = "Attack";
        attackAction.FieldCard = attacker;
        attackAction.Player = attacker.Player;

        List <ITargetable> newTargets = Targeting.ConvertTargets (attackAction.TargetTypes, targets, attacker.Player);

        if (attackAction.TragetVaildity (newTargets)) {
            attackAction.SetTargets (newTargets);
            AddTargetorToStack (attackAction);

            CallEffects ();

            hasActed = true;
        }
    }

    public void FieldCardAct (FieldCard actor, List<ITargetable> extraCostTargets, List<ITargetable> targets, ActionAbility action) {
        if (!IsServer) return;
        
        if (!actor.Player.Equals(playerPriority)) return;
        if (action.Speed == TargetorPriority.Ritual && targetorStack.Count > 0) return;


        if (actor.Player.CurrentMana < action.ManaCost) return;
        if (action.UsesActionPoint && actor.currActionPoints.Value == 0) return; else actor.ConsumeActionPoint ();
        actor.Player.SpendMana (action.ManaCost);
        
        if (action.TragetVaildity (targets)) {
            action.SetTargets (targets);
            AddTargetorToStack (action);
            hasActed = true;
        } else {Debug.Log("Action Ability failed targeting");};
    }

    public bool CheckCanAttack (FieldUnit attacker, FieldUnit target) { //Returns true if this card fits all the conditions to attack a target
        if (attacker.Player.FieldUnits.Contains (target)) return false; //Return if target unit is a friendly.
        //Check for Menacing keyword.
        if (target.UnitsCard.UnitCard.StaticKeywords.HasFlag (UnitCardStaticKeywords.Menacing) && attacker.strength.Value < target.strength.Value) return false;
        //Check for Stealth keyword.
        if (target.UnitsCard.UnitCard.StaticKeywords.HasFlag (UnitCardStaticKeywords.Stealth) && !attacker.UnitsCard.UnitCard.StaticKeywords.HasFlag (UnitCardStaticKeywords.Scout)) return false;

        return true;
    }

    public void ButtonPress (Player player) {
        if (!IsServer) return;
        if (!player.Equals (playerPriority)) return;

        if (targetorStack.Count == 0) PassTurn (player); //If there is nothing on the stack, just pass the turn.
        else { //Else...
            if (hasActed) //If the player has acted during their priority 
                SwitchPriority ();  //We switch priorities.
            else
                CallTargetors (); //Else, we just call the effects.
        }
    }


    void PassTurn (Player player) {
        if (!IsServer) return;

        //If the player requesting the pass is the player who has the turn... 
        if (player.Equals(playerTurn)) {            
            if (turnNumber.Value % 2 == 0)
                playerTurn = player1;
            else
                playerTurn = player2;

            playerPriority = playerTurn;
            PassTurnClientRPC (playerTurn.OwnerClientId);
            SwitchPriorityClientRpc (playerPriority.OwnerClientId);

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


    void SwitchPriority () {
        if (!IsServer) return;

        if (playerPriority.Equals (player1))
            playerPriority = player2;
        else 
            playerPriority = player1;

        hasActed = false;

        SwitchPriorityClientRpc (playerPriority.OwnerClientId);
    }
    [ClientRpc]
    void SwitchPriorityClientRpc (ulong netid) {
        if (NetworkManager.Singleton.LocalClientId == netid)
            localPlayerPriority = true;
        else 
            localPlayerPriority = false;
    }

    public void AddEffectToStack (CardEffect effect) {
        if (!IsServer) return;

        effectStack.Push (effect);
    }

    //Call the effect stack here. Called recursively.
    public void CallEffects () {
        if (!IsServer) return;

        if (effectStack.Count > 0) {
            CardEffect effect = effectStack.Pop();
            
            effect.DoEffect ();
            Debug.Log ("Effect: " + effect);
            
            CallEffects ();
        }
    }

    public void AddTargetorToStack (Targetor targetor) {
        if (!IsServer) return;

        targetorStack.Add (targetor);
        UpdateStackUI ();
    }

    //Call the top-most targetor on the stack.
    void CallTargetors () {
        if (!IsServer) return;

        if (targetorStack.Count > 0) {
            Targetor targetor = targetorStack[targetorStack.Count-1];
            targetorStack.RemoveAt (targetorStack.Count-1);

            if (targetor.TragetVaildity (targetor.Targets)) {
                targetor.DoEffect ();
                Debug.Log ("Targetor: " + targetor.name);

                if (targetor is Spell) targetor.Player.SpellResolved ((targetor as Spell).SpellCard);
            }

            CallEffects ();
            UpdateStackUI ();
        }

        if (targetorStack.Count == 0) {
            playerPriority = playerTurn;
            
            SwitchPriorityClientRpc (playerPriority.OwnerClientId);
            UpdateStackUI ();
        }
    }

    void UpdateStackUI () {
        if (!IsServer) return;

        string[] stackLocations = new string [targetorStack.Count];
        ulong[] playerIDs = new ulong [targetorStack.Count];
        //Vector2[][] targets = new Vector2 [targetorStack.Count][];

        for (int i = 0; i < targetorStack.Count; i++) { 
            stackLocations[i] = targetorStack[i].Location();
            Debug.Log (stackLocations[i]);
            playerIDs[i] = targetorStack[i].Player.OwnerClientId;
        }

        player1.UpdateStackClientRPC (stackLocations, playerIDs);
        player2.UpdateStackClientRPC (stackLocations, playerIDs);
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

    public bool LocalPlayerPriority {
        get {
            return localPlayerPriority;
        }
    }

    public Player Player1 {get {return player1;}}
    public Player Player2 {get {return player2;}}

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

    public int TurnNumber {get {return turnNumber.Value;}}

}
