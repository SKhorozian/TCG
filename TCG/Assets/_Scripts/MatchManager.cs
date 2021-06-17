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

    [SerializeField] GameObject fieldHeroPrefab;
    [SerializeField] GameObject fieldUnitPrefab;
    [SerializeField] GameObject fieldStructurePrefab;

    Stack<CardEffect> effectStack = new Stack<CardEffect> ();

    FieldState fieldState;

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

        fieldState = new FieldState ();
        fieldState.ChangeState(CurrentFieldState.FreePlay); //Later on offer both players mulligans

        turnNumber.Value = 1;

        playerTurn = player1;
        localPlayerTurn = true;

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

        if (IsServer) SummonHeros (); 
    }

    //Summon the heroes as Field Cards
    public void SummonHeros () {
        if (!IsServer) return;
        
        //Player 1:
        GameObject player1Hero = NetworkManager.Instantiate (fieldHeroPrefab, Vector3.zero, Quaternion.identity);
        player1Hero.gameObject.GetComponent<NetworkObject> ().SpawnWithOwnership (player1.OwnerClientId, null, true);

        HexagonCell cell1;

        if (fieldGrid.Cells.TryGetValue (new Vector2 (5, 17), out cell1)) {
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

        if (fieldGrid.Cells.TryGetValue (new Vector2 (5, 1), out cell2)) {
            HeroCardInstance heroCardInstance = new HeroCardInstance (player2.StartDeck.HeroCard);
            FieldHero fieldHero = player2Hero.GetComponent<FieldHero> ();
            fieldHero.SummonHero (heroCardInstance, player2, cell1);

            fieldHero.position.Value = cell2.gameObject.transform.position;
            cell2.FieldCard = fieldHero;

            player2.SummonHero (fieldHero);
        } else {
            throw new System.Exception ("Invalid Hex Cell for player 2's hero!");
        }

        CallEffects ();

    }


    //Returns true if the card was played successfully, else return false
    public bool PlayCard (CardInstance card, Player player, Vector2 placement, Vector2[] targets, Vector2[] extraCostTargets) {
        if (!IsServer) return false;

        if (!player.Equals(playerTurn)) return false;
        if (player.CurrentMana < card.Cost) return false;

        //Extra Cost
        if (card.Card.ExtraCost) {
            ExtraCost extraCost = Instantiate <ExtraCost> (card.Card.ExtraCost);
            
            List<ITargetable> newTargets = Targeting.ConvertTargets (extraCost.TargetTypes, extraCostTargets, player);

            if (extraCost.TragetVaildity(newTargets, player)) { //If targets are valid, then we proceed to play the card. Otherwise, we return.
                extraCost.SetTargets (newTargets);
                extraCost.DoEffect ();
                CallEffects ();
            } else {Debug.Log("Extra Cost failed targeting"); return false;};
        }

        switch (card.Type) { 
            case CardType.Unit: {
                if (placement == null) return false;
                if (!(card is UnitCardInstance)) return false;

                HexagonCell cell;
                if (!fieldGrid.Cells.TryGetValue(placement, out cell)) {Debug.Log ("Placement invalid"); return false;};
                if (cell.FieldCard) return false; //If the cell is occupied return false

                //Spend Mana
                player.SpendMana (card.Cost);

                FieldUnit summonedUnit = SummonUnit (card, player, cell);

                //Play Effects
                if ((card as UnitCardInstance).UnitCard.OnPlayEffect) {
                    PlayAbility playEffect = Instantiate <PlayAbility> ((card as UnitCardInstance).UnitCard.OnPlayEffect);
                    playEffect.FieldCard = summonedUnit;
            
                    List<ITargetable> newTargets = Targeting.ConvertTargets (playEffect.TargetTypes, targets, player);

                    if (playEffect.TragetVaildity(newTargets, player) && summonedUnit) { //If targets are valid, then we proceed to call the play effect. Otherwise, we ignore it.
                        playEffect.SetTargets (newTargets);
                        playEffect.DoEffect ();
                        CallEffects ();
                    } else {Debug.Log("Play Effect failed targeting");};
                }

                Debug.Log ("Player " + player.OwnerClientId + " played " + card.CardName + " on cell " + placement);
            } break;
            case CardType.Structure: {
                if (placement == null) return false;
                if (!(card is StructureCardInstance)) return false;

                HexagonCell cell;
                if (fieldGrid.Cells.TryGetValue(placement, out cell)) {Debug.Log ("Placement invalid"); return false;};
                if (cell.FieldCard) return false; //If the cell is occupied return false

                //Spend Mana
                player.SpendMana (card.Cost);

                FieldStructure summonedStructure = SummonStructure (card, player, cell);

                //Play Effects
                if ((card as StructureCardInstance).StructureCard.OnPlayEffect) {
                    PlayAbility playEffect = Instantiate <PlayAbility> ((card as StructureCardInstance).StructureCard.OnPlayEffect);
                    playEffect.FieldCard = summonedStructure;
            
                    List<ITargetable> newTargets = Targeting.ConvertTargets (playEffect.TargetTypes, targets, player);

                    if (playEffect.TragetVaildity(newTargets, player) && summonedStructure) { //If targets are valid, then we proceed to call the play effect. Otherwise, we ignore it.
                        playEffect.SetTargets (newTargets);
                        playEffect.DoEffect ();
                        CallEffects ();
                    } else {Debug.Log("Play Effect failed targeting");};
                }


                Debug.Log ("Player " + player.OwnerClientId + " played " + card.CardName + " on cell " + placement);
            } break;
            case CardType.Spell:
                if (!(card is SpellCardInstance)) return false;

                //Spell Effects
                if ((card as SpellCardInstance)._SpellCard.Spell) {
                    Spell spell = Instantiate <Spell> ((card as SpellCardInstance)._SpellCard.Spell);
                    
                    spell.SpellCard = card as SpellCardInstance;
                    spell.Player = player;
            
                    List<ITargetable> newTargets = Targeting.ConvertTargets (spell.TargetTypes, targets, player);

                    if (spell.TragetVaildity(newTargets, player)) { //If targets are valid, then we proceed to call the spell effect. Otherwise, the spell doesn't go off.
                        spell.SetTargets (newTargets);
                        spell.DoEffect ();
                        CallEffects ();
                    } else {Debug.Log("Spell failed targeting"); return false;};
                }

                //Spend Mana
                player.SpendMana (card.Cost);

                Debug.Log ("Player " + player.OwnerClientId + " casted " + card.CardName);
                break;
            default:
                break;
        }

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

        CallEffects ();
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
        
        CallEffects ();
        return fieldUnit;
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
        if (!IsServer) return;

        if (playerTurn.OwnerClientId != netid) return;
        if (unit.OwnerClientId != netid) return;

        MovementAction moveAction = new MovementAction ();
        moveAction.FieldCard = unit;

        HexagonCell hexagonCell;
        if (fieldGrid.Cells.TryGetValue (cell, out hexagonCell)) {

            List<ITargetable> targets = new List<ITargetable> ();
            targets.Add (hexagonCell);

            if (moveAction.TragetVaildity (targets, unit.Player)) {
                moveAction.SetTargets (targets);
                moveAction.DoEffect ();

                CallEffects ();
            }
        }

 
    }

    public void UnitAttack (FieldUnit attacker, Vector2 cell, ulong netid) {
        if (!IsServer) return;

        if (playerTurn.OwnerClientId != netid) return;
        if (attacker.OwnerClientId != netid) return;
        if (HexagonMetrics.GetDistantce (attacker.Cell.Position, cell) > attacker.attackRange.Value) return;
        if (attacker.currActionPoints.Value <= 0) return;

        HexagonCell hexCell;

        if (attacker.Player.MatchManage.FieldGrid.Cells.TryGetValue (cell, out hexCell)) {
            if (hexCell.FieldCard) {
                if (hexCell.FieldCard is FieldUnit)  {

                    FieldUnit target = hexCell.FieldCard as FieldUnit;

                    if (!CheckCanAttack (attacker, target)) return;

                    attacker.Strike (target);
                    //The target strikes back if it has an action point and is within its range
                    if (target.currActionPoints.Value > 0 && (HexagonMetrics.GetDistantce (attacker.Cell.Position, target.Cell.Position) <= target.attackRange.Value)) target.Strike (attacker); 
                    
                    attacker.ConsumeActionPoint (1);

                } else if (hexCell.FieldCard is FieldHero) {

                    FieldHero target = hexCell.FieldCard as FieldHero;

                    if (attacker.Player.FieldHero.Equals (target)) return; //Return if target hero is a friendly.
                    attacker.Strike (target);
                    
                    attacker.ConsumeActionPoint (1);
                } else {
                    return;
                }
            }
        }

        CallEffects ();
    }

    public bool CheckCanAttack (FieldUnit attacker, FieldUnit target) { //Returns true if this card fits all the conditions to attack a target
        if (attacker.Player.FieldUnits.Contains (target)) return false; //Return if target unit is a friendly.
        if (target.UnitsCard.UnitCard.StaticKeywords.HasFlag (UnitCardStaticKeywords.Menacing) && attacker.strength.Value < target.strength.Value) return false;
        if (target.UnitsCard.UnitCard.StaticKeywords.HasFlag (UnitCardStaticKeywords.Stealth) && !attacker.UnitsCard.UnitCard.StaticKeywords.HasFlag (UnitCardStaticKeywords.Scout)) return false;

        return true;
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


    public void AddEffectToStack (CardEffect effect) {
        effectStack.Push (effect);
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

}
