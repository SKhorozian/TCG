using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Player player;

    [SerializeField] PlayerHandDisplay handDisplay;
    [SerializeField] TurnButton turnButton;
    
    [SerializeField] FieldUnit focusUnit;
    [SerializeField] CardHandController focusCard;


    [SerializeField] Targetor targetor;
    [SerializeField] ExtraCost extraCost;

    [Space (10), SerializeField] Vector2 placement;
    [SerializeField] Vector2[] targets;
    [SerializeField] Vector2[] extraCostTargets;

    [SerializeField] int selectedTargets;
    [SerializeField] int selectedExtraCostTargets;

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
        if (focusCard) {
            focusUnit = null;

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
                focusCard.DeFocus ();
                focusCard = null;
            }

        } else if (focusUnit) {

            if (Input.GetButtonDown ("Fire1")) {

                int layerMask = 1 << 6;

                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

                if (Physics.Raycast (ray, out hit, Mathf.Infinity, layerMask)) {
                    HexagonCell cell = hit.transform.GetComponent<HexagonCell> ();
                    focusUnit.TargetCell (cell.Position, NetworkManager.Singleton.LocalClientId);
                }

                focusUnit = null;
            } else if (Input.GetButtonDown ("Fire2")) {
                focusUnit = null;
            }

        } else {

            if (Input.GetButtonDown ("Fire1")) {
                int layerMask = 1 << 7;

                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

                if (Physics.Raycast (ray, out hit, Mathf.Infinity, layerMask)) {
                    if (hit.transform.GetComponent<NetworkObject>().IsOwner ) {
                        FocusUnit (hit.transform.gameObject.GetComponent<FieldUnit> ());
                    }
                }
            }


        }

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
                        if (targetor.TragetVaildity(selectedTargets, cell, player)) {
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

                        if (targetor.TragetVaildity(selectedTargets, fieldCard, player)) {
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
                        if (extraCost.TragetVaildity(selectedExtraCostTargets, cell, player)) {
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

                        if (extraCost.TragetVaildity(selectedExtraCostTargets, fieldCard, player)) {
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
        if ((focusCard.cardInstance.Type == CardType.Unit || focusCard.cardInstance.Type == CardType.Structure) && placement.Equals (Vector2.zero)) return;

        if (extraCost)
            if (extraCost.TargetTypes.Length != selectedExtraCostTargets) return;

        if (targetor)
            if (targetor.TargetTypes.Length != selectedTargets) return;
        
        focusCard.PlayCard (placement, targets, extraCostTargets);
        focusCard.DeFocus ();
        focusCard = null;
    }

    public void UpdateCardDisplays (CardInstance[] instances) {
        handDisplay.UpdateCardDisplays (instances);
    }

    public void FocusCard (CardHandController card) {
        if (focusCard)
            focusCard.DeFocus ();

        targetor = null;
        extraCost = null;

        targets = null;
        selectedTargets = 0;
        extraCostTargets = null;
        selectedExtraCostTargets = 0;
        placement = Vector2.zero;

        focusCard = card;
        focusUnit = null;

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

    public void FocusUnit (FieldUnit unit) {
        targets = null;
        selectedTargets = 0;
        placement = Vector2.zero;

        focusUnit = unit;
        focusCard = null;
    }

    public void TargetCard (int cardNumber, CardInstance cardInstance) {

    }

}
