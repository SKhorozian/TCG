using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerHandDisplay handDisplay;
    [SerializeField] TurnButton turnButton;
    
    [SerializeField] FieldUnit focusUnit;
    [SerializeField] CardDisplay focusCard;

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
                        focusCard.PlayCard ();
                        focusCard.DeFocus ();
                        focusCard = null;
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

    public void UpdateCardDisplays (CardInstance[] instances) {
        handDisplay.UpdateCardDisplays (instances);
    }

    public void FocusCard (CardDisplay card) {
        if (focusCard)
            focusCard.DeFocus ();
        focusCard = card;
        focusUnit = null;
    }

    public void FocusUnit (FieldUnit unit) {
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
