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
    }

    public void Hover() {
        //rectT.localPosition = new Vector3 (rectT.localPosition.x,150,0); 
    }

    public void Lower() {
        //rectT.localPosition = new Vector3 (rectT.localPosition.x,0,0); 
    }

    public void StartDrag () {
        originalPos = rectT.position;
    }

    public void Drag() {
        rectT.position = Input.mousePosition;
    }

    public void Drop() {
        playerController.FocusCard (this);
    }

    public void DeFocus () {
        rectT.position = originalPos;
    }

    public void PlayCard (Vector2[] fieldTargets, int[] handTargets, int[] stackTargets) {

        //Play the card
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkedClient))
        {
            var player = networkedClient.PlayerObject.GetComponent<Player>();
            if (player)
            {
                player.PlayCard (handNumber, fieldTargets, handTargets, stackTargets);
            }
        }


    }

    public CardInstance cardInstance {get {return cardDisplay.cardInstance;}}
}
