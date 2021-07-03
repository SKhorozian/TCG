using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Player player;

    [SerializeField] PlayerHandDisplay handDisplay;
    [SerializeField] TurnButton turnButton;
    
    [SerializeField] FieldCard focusFieldCard;

    [SerializeField] CardHandController focusCard;

    [SerializeField] Targetor targetor;
    [SerializeField] ExtraCost extraCost;

    [Space (10), SerializeField] Vector2 placement;
    [SerializeField] Vector2[] targets;
    [SerializeField] Vector2[] extraCostTargets;

    [SerializeField] int selectedTargets;
    [SerializeField] int selectedExtraCostTargets;

    [Space (10)]
    [SerializeField] CardDisplay hoverDisplay;
    [SerializeField] GameObject hoverobject;

    [Space (10)]
    [SerializeField] CardDisplay cardFocusDisplay;
    [SerializeField] GameObject cardFocusParent;

    [Space (10)]
    [SerializeField] GameObject fieldCardActions;
    [SerializeField] GameObject attackButton;
    [SerializeField] GameObject moveButton;
    [SerializeField] GameObject act1Button;
    [SerializeField] GameObject act2Button;
    [SerializeField] GameObject act3Button;

    [SerializeField] int actionN = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkedClient))
        {
            player = networkedClient.PlayerObject.GetComponent<Player>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            CheckForCardHighlight ();

        if (focusCard) {

            //If targeting is fulfilled then play the card
            PlayCard (); 

            //Targeting done here
            if (Input.GetButtonDown ("Fire1")) {  
                if ((focusCard.cardInstance.Type == CardType.Unit || focusCard.cardInstance.Type == CardType.Structure) && placement.Equals (Vector2.zero)) { //If it is a unit or structure, we need to ask for its placement.
                    TargetPlacement ();
                } else if (extraCost) {
                    ExtraCostTargeting ();
                } else if (targetor) {
                    TargetorTargeting ();
                }
            } else if (Input.GetButtonDown ("Fire2")) { //If we right click, defocus the card.
                DefocusCard ();
            }

        } else if (focusFieldCard) {

            if (Input.GetButtonDown ("Fire1")) {

                PerformAction ();

                if (targetor) {
                    TargetorTargeting ();
                    PerformAction ();
                }

            } else if (Input.GetButtonDown ("Fire2")) {
                DefocusFieldCard ();
            }

        } else {

            if (Input.GetButtonDown ("Fire1")) {
                int layerMask = 1 << 7;

                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

                if (Physics.Raycast (ray, out hit, Mathf.Infinity, layerMask)) {
                    if (hit.transform.GetComponent<NetworkObject>().IsOwner ) {
                        FocusFieldCard (hit.transform.gameObject.GetComponent<FieldUnit> ());
                    }
                }
            }


        }

    }

    public void SetTargetorToAttack () {
        if (!focusFieldCard) return;

        ResetTargets ();

        AttackAction attackAction = new AttackAction ();
        attackAction.FieldCard = focusFieldCard;
        attackAction.Player = player;

        targetor = attackAction;
        targets = new Vector2[targetor.TargetTypes.Length];
        
        actionN = 3;
    }

    public void SetTargetorToMove () {
        if (!focusFieldCard) return;

        ResetTargets ();

        MovementAction movementAction = new MovementAction ();
        movementAction.FieldCard = focusFieldCard;
        movementAction.Player = player;

        targetor = movementAction;
        targets = new Vector2[targetor.TargetTypes.Length];

        actionN = 4;
    }

    public void SetTargetorToAction (int n) {
        if (!focusFieldCard) return;
        if (n > focusFieldCard.Actions.Length) return;

        ResetTargets ();

        ActionAbility action = Instantiate <ActionAbility> (focusFieldCard.Actions[n]);
        action.FieldCard = focusFieldCard;
        action.Player = player;

        targetor = action;
        targets = new Vector2[targetor.TargetTypes.Length];

        actionN = n;
    }

    public void PerformAction () {
        if (focusFieldCard == null) return;
        if (targetor == null) return; 

        if (targetor)
            if (targetor.TargetTypes.Length != selectedTargets) return;

        if (actionN == 3) {
            if (focusFieldCard is FieldUnit)
                (focusFieldCard as FieldUnit).Attack (targets, player.OwnerClientId);
        } else if (actionN == 4) {
            if (focusFieldCard is FieldUnit)
                (focusFieldCard as FieldUnit).MoveUnit (targets, player.OwnerClientId);
        } else {
            focusFieldCard.PerformAction (actionN, targets);
        }

        DefocusFieldCard ();
    }


    void TargetPlacement () {
        int layerMask = 1 << 6;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

        if (Physics.Raycast (ray, out hit, Mathf.Infinity, layerMask)) {
            HexagonCell cell = hit.transform.GetComponent<HexagonCell> ();
            placement = cell.Position;
        }
    }

    void TargetorTargeting () {
        if (selectedTargets < targetor.TargetTypes.Length) {
            switch (targetor.TargetTypes[selectedTargets]) {
                case TargetType.Hex: {
                    int layerMask = 1 << 6;

                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

                    if (Physics.Raycast (ray, out hit, Mathf.Infinity, layerMask)) {
                        HexagonCell cell = hit.transform.GetComponent<HexagonCell> ();
                        if (targetor.TragetVaildity(selectedTargets, cell)) {
                            targets [selectedTargets] = cell.Position;
                            selectedTargets++;
                            PlayCard ();
                        }
                    }
                }
                    break;
                case TargetType.FieldCard: {
                    int layerMask = 1 << 7;

                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

                    if (Physics.Raycast (ray, out hit, Mathf.Infinity, layerMask)) { 
                        FieldCard fieldCard = hit.transform.GetComponent<FieldCard> ();

                        if (targetor.TragetVaildity(selectedTargets, fieldCard)) {
                            int layerMaskC = 1 << 6;

                            RaycastHit hitC;
                            Ray rayC = Camera.main.ScreenPointToRay (Input.mousePosition);

                            if (Physics.Raycast (rayC, out hitC, Mathf.Infinity, layerMaskC)) {
                                HexagonCell cell = hitC.transform.GetComponent<HexagonCell> ();
                                targets [selectedTargets] = cell.Position;
                                selectedTargets++;
                                PlayCard ();
                            }
                        }
                    }
                }
                    break;
                case TargetType.Hand: {
                    //Wait for the player to click on a card in hand
                }
                    break;
                case TargetType.Trap: {

                }
                    break;
                case TargetType.Stack: {

                }
                    break;
            }
        }
    }

    void ExtraCostTargeting () {
        if (selectedExtraCostTargets < extraCost.TargetTypes.Length) {
            switch (extraCost.TargetTypes[selectedExtraCostTargets]) {
                case TargetType.Hex: {
                    int layerMask = 1 << 6;

                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

                    if (Physics.Raycast (ray, out hit, Mathf.Infinity, layerMask)) {
                        HexagonCell cell = hit.transform.GetComponent<HexagonCell> ();
                        if (extraCost.TragetVaildity(selectedExtraCostTargets, cell)) {
                            extraCostTargets [selectedExtraCostTargets] = cell.Position;
                            selectedExtraCostTargets++;
                        }
                    }
                }
                    break;
                case TargetType.FieldCard: {
                    int layerMask = 1 << 7;

                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

                    if (Physics.Raycast (ray, out hit, Mathf.Infinity, layerMask)) { 
                        FieldCard fieldCard = hit.transform.GetComponent<FieldCard> ();

                        if (extraCost.TragetVaildity(selectedExtraCostTargets, fieldCard)) {
                            int layerMaskC = 1 << 6;

                            RaycastHit hitC;
                            Ray rayC = Camera.main.ScreenPointToRay (Input.mousePosition);

                            if (Physics.Raycast (rayC, out hitC, Mathf.Infinity, layerMaskC)) {
                                HexagonCell cell = hitC.transform.GetComponent<HexagonCell> ();
                                extraCostTargets [selectedExtraCostTargets] = cell.Position;
                                selectedExtraCostTargets++;
                            }
                        }
                    }
                }
                    break;
                case TargetType.Hand: {
                    //Wait for the player to click on a card in hand
                }
                    break;
                case TargetType.Trap: {

                }
                    break;
                case TargetType.Stack: {

                }
                    break;
            }
        }
    }

    void PlayCard () {
        if (focusCard == null) return;

        if ((focusCard.cardInstance.Type == CardType.Unit || focusCard.cardInstance.Type == CardType.Structure) && placement.Equals (Vector2.zero)) return;

        if (extraCost)
            if (extraCost.TargetTypes.Length != selectedExtraCostTargets) return;

        if (targetor)
            if (targetor.TargetTypes.Length != selectedTargets) return;
        
        focusCard.PlayCard (placement, targets, extraCostTargets);
        DefocusCard ();
    }

    public void UpdateCardDisplays (CardInstance[] instances) {
        handDisplay.UpdateCardDisplays (instances);
    }

    void CheckForCardHighlight () {
        int layerMask = 1 << 7;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

        if (Physics.Raycast (ray, out hit, Mathf.Infinity, layerMask)) { 
            FieldCard fieldCard = hit.transform.GetComponent<FieldCard> ();

            hoverobject.SetActive (true);
            hoverDisplay.SetCard (fieldCard.Card);

            hoverobject.transform.position = Input.mousePosition;
        } else {
            hoverobject.SetActive (false);
        }
    }

    public void FocusCard (CardHandController card) {
        if (focusCard)
            DefocusCard ();

        DefocusFieldCard ();

        ResetTargets ();

        focusCard = card;

        //Card Visuals
        cardFocusParent.SetActive (true);
        cardFocusDisplay.SetCard (card.cardInstance);

        extraCost = card.cardInstance.Card.ExtraCost;

        if ( card.cardInstance.Card is UnitCard ) {
            targetor = (card.cardInstance.Card as UnitCard).OnPlayEffect;
        } else if (card.cardInstance.Card is StructureCard) {
            targetor = (card.cardInstance.Card as StructureCard).OnPlayEffect;
        } else if (card.cardInstance.Card is SpellCard) {
            targetor = (card.cardInstance.Card as SpellCard).Spell;
        } 

        if (targetor)
            targets = new Vector2[targetor.TargetTypes.Length];

        if (extraCost)
            extraCostTargets = new Vector2[extraCost.TargetTypes.Length];

        PlayCard ();
    }

    void DefocusCard () {
        focusCard.DeFocus ();
        focusCard = null;
        cardFocusParent.SetActive (false);
        
        ResetTargets ();
    }

    public void FocusFieldCard (FieldCard unit) {
        focusFieldCard = unit;

        ResetTargets ();

        fieldCardActions.SetActive(true);

        attackButton.gameObject.SetActive (false);
        moveButton.gameObject.SetActive (false);
        act1Button.gameObject.SetActive (false);
        act2Button.gameObject.SetActive (false);
        act3Button.gameObject.SetActive (false);

        if (focusFieldCard is FieldUnit) {
            attackButton.gameObject.SetActive (true);
            moveButton.gameObject.SetActive (true);

            FieldUnit fieldUnit = (focusFieldCard as FieldUnit);

            if (fieldUnit.UnitsCard.UnitCard.Actions == null) return;

            if (fieldUnit.UnitsCard.UnitCard.Actions.Count == 1) act1Button.gameObject.SetActive (true);
            if (fieldUnit.UnitsCard.UnitCard.Actions.Count == 2) act2Button.gameObject.SetActive (true);
            if (fieldUnit.UnitsCard.UnitCard.Actions.Count == 3) act3Button.gameObject.SetActive (true);
        } else if (focusFieldCard is FieldStructure) {
            FieldStructure fieldStructure = (focusFieldCard as FieldStructure);

            if (fieldStructure.StructursCard.StructureCard.Actions == null) return;

            if (fieldStructure.StructursCard.StructureCard.Actions.Count == 1) act1Button.gameObject.SetActive (true);
            if (fieldStructure.StructursCard.StructureCard.Actions.Count == 2) act2Button.gameObject.SetActive (true);
            if (fieldStructure.StructursCard.StructureCard.Actions.Count == 3) act3Button.gameObject.SetActive (true);
        } else if (focusFieldCard is FieldHero) {
            FieldHero fieldHero = (focusFieldCard as FieldHero);

            if (fieldHero.HeroCard.HeroCard.Actions == null) return;

            if (fieldHero.HeroCard.HeroCard.Actions.Count == 1) act1Button.gameObject.SetActive (true);
            if (fieldHero.HeroCard.HeroCard.Actions.Count == 2) act2Button.gameObject.SetActive (true);
            if (fieldHero.HeroCard.HeroCard.Actions.Count == 3) act3Button.gameObject.SetActive (true);
        }



    }

    void DefocusFieldCard () {
        focusFieldCard = null;

        fieldCardActions.SetActive(false);

        ResetTargets ();
    }

    public void ResetTargets () {
        targetor = null;
        extraCost = null;

        targets = null;
        selectedTargets = 0;
        extraCostTargets = null;
        selectedExtraCostTargets = 0;
        placement = Vector2.zero;
    }

    public void TargetCard (int cardNumber, CardInstance cardInstance) {

    }

}
