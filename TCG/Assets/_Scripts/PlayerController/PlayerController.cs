using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerHandDisplay handDisplay;
    [SerializeField] TurnButton turnButton;
    
    [SerializeField] FieldUnit focusUnit;
    [SerializeField] CardHandController focusCard;


    [SerializeField] OnPlay currPlay;

    [SerializeField] List<Vector2> fieldTargets; 
    [SerializeField] List<Vector2> trapTargets;
    [SerializeField] List<int> handTargets;
    [SerializeField] List<int> stackTargets;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (focusCard) {
            focusUnit = null;
            switch (focusCard.cardInstance.Type) {
                case CardType.Unit:

                    if (Input.GetButtonDown ("Fire1")) {
                        
                        /////////////////////
                        if (fieldTargets.Count == 0) { 
                            int layerMask = 1 << 6;

                            RaycastHit hit;
                            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

                            if (Physics.Raycast (ray, out hit, Mathf.Infinity, layerMask)) {
                                HexagonCell cell = hit.transform.GetComponent<HexagonCell> ();
                                fieldTargets.Add (cell.Position);
                            }
                        } else if (currPlay) {
                            if (fieldTargets.Count < currPlay.FieldTargetsCount + 1) {
                                int layerMask = 1 << 6;

                                RaycastHit hit;
                                Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

                                if (Physics.Raycast (ray, out hit, Mathf.Infinity, layerMask)) {
                                    HexagonCell cell = hit.transform.GetComponent<HexagonCell> ();
                                    fieldTargets.Add (cell.Position);
                                }
                            }
                        }
                        /////////////////////


                        if (fieldTargets.Count > 0) {
                            if (!currPlay || ValidateTargets ()) {
                                focusCard.PlayCard (fieldTargets.ToArray (), handTargets.ToArray (), stackTargets.ToArray ());
                                focusCard.DeFocus ();
                                focusCard = null;
                            }
                        }

                    } else if (Input.GetButtonDown ("Fire2")) {
                        focusCard.DeFocus ();
                        focusCard = null;
                    }
                    
                    break;
                case CardType.Structure:
                
                    if (Input.GetButtonDown ("Fire1")) {
                        if (fieldTargets.Count == 0) { 
                            int layerMask = 1 << 6;

                            RaycastHit hit;
                            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

                            if (Physics.Raycast (ray, out hit, Mathf.Infinity, layerMask)) {
                                HexagonCell cell = hit.transform.GetComponent<HexagonCell> ();
                                fieldTargets.Add (cell.Position);
                            }
                        }

                        if (fieldTargets.Count > 0) {
                            focusCard.PlayCard (fieldTargets.ToArray (), handTargets.ToArray (), stackTargets.ToArray ());
                            focusCard.DeFocus ();
                            focusCard = null;
                        }
                    } else if (Input.GetButtonDown ("Fire2")) {
                        focusCard.DeFocus ();
                        focusCard = null;
                    }

                    break;
                default:
                    focusCard.DeFocus ();
                    focusCard = null;
                    break;
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

    public bool ValidateTargets () {

        if (fieldTargets.Count != currPlay.FieldTargetsCount + 1) return false;
        if (handTargets.Count != currPlay.HandTargetsCount) return false;
        if (stackTargets.Count != currPlay.StackTargetsCount) return false;
        
        return true;
    }


    public void UpdateCardDisplays (CardInstance[] instances) {
        handDisplay.UpdateCardDisplays (instances);
    }

    public void FocusCard (CardHandController card) {
        if (focusCard)
            focusCard.DeFocus ();

        currPlay = null;
        fieldTargets = new List<Vector2> ();
        handTargets = new List<int> ();
        stackTargets = new List<int> ();
        trapTargets = new List<Vector2> ();

        focusCard = card;
        focusUnit = null;

        if ( (card.cardInstance.Card is UnitCard) ) {
            currPlay = (card.cardInstance.Card as UnitCard).UnitOnPlayEffect;
        }


    }

    public void FocusUnit (FieldUnit unit) {
        fieldTargets = new List<Vector2> ();
        handTargets = new List<int> ();
        stackTargets = new List<int> ();
        trapTargets = new List<Vector2> ();

        focusUnit = unit;
        focusCard = null;
    }

    public void OnDrawGizmos () {
        if (focusUnit) {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere (focusUnit.position.Value, focusUnit.movementSpeed.Value);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere (focusUnit.position.Value, focusUnit.attackRange.Value);

        } else if (focusCard) {

        }
    }
}
