using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class CardHandController : MonoBehaviour
{
    [SerializeField] RectTransform rectT;
    [SerializeField] Vector3 originalPos;
    [SerializeField] Vector3 focusPosition;
    [SerializeField] PlayerController playerController;
    [SerializeField] int handNumber;

    [SerializeField] CardDisplay cardDisplay;

    bool isFocus = false;

    void Start () {
        originalPos = rectT.position;
    }

    public void SetCard (CardInstance cardInstance) {
        cardDisplay.SetCard (cardInstance);
        cardDisplay.gameObject.SetActive (true);
    }

    public void TargetCard () {
        playerController.TargetCard (handNumber, cardInstance);
    }

    public void StartDrag () {
        originalPos = rectT.position;
    }

    public void Drag() {
        rectT.position = Input.mousePosition;
    }

    public void Drop() {
        if (Input.mousePosition.y > Screen.height/3) {
            playerController.FocusCard (this);
            cardDisplay.gameObject.SetActive (false);
        }
        else {
            DeFocus ();
            cardDisplay.gameObject.SetActive (true);
        }
    }

    public void DeFocus () {
        rectT.position = originalPos;
        cardDisplay.gameObject.SetActive (true);
    }

    public void PlayCard (Vector2 placement, Vector2[] targets, Vector2[] extraCostTargets) {

        //Play the card
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkedClient))
        {
            var player = networkedClient.PlayerObject.GetComponent<Player>();
            if (player)
            {
                player.PlayCard (handNumber, placement, targets, extraCostTargets);
            }
        }
    }

    public CardInstance cardInstance {get {return cardDisplay.cardInstance;}}
    public int HandNumber {get {return handNumber;}}
}
