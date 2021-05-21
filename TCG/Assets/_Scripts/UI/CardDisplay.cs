using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MLAPI;

public class CardDisplay : MonoBehaviour
{
    [SerializeField] CardInstance card;
    [SerializeField] int handNumber;

    [SerializeField] RectTransform rectT;
    [SerializeField] Vector3 originalPos;

    [SerializeField] Image cardColor;
    [SerializeField] Image cardArt;

    [SerializeField] TextMeshProUGUI cardName;
    [SerializeField] TextMeshProUGUI cost;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI strength;
    [SerializeField] TextMeshProUGUI health;

    [SerializeField] Color[] colors;

    // Start is called before the first frame update
    void Start()
    {
        UpdateCard();
    }

    void UpdateCard () {
        if (card != null) {

            cardName.text = card.CardName;

            if (card.CardName.Length > 8)
                cardName.fontSize = 15f;
            else 
                cardName.fontSize = 20f;

            description.text = card.Description;
            cost.text = card.Cost.ToString();

            switch (card.Type) {
                case CardType.Unit:
                    if (card.Card.GetType ().Name.Equals ("UnitCard")) {
                        UnitCardInstance unitCard = (UnitCardInstance)card;
                        strength.text = unitCard.Strength.ToString();
                        health.text = unitCard.Health.ToString();   
                    }
                break;
                default:
                break;
            }


            cardArt.sprite = card.CardArt;

            switch (card.Color) {
                case CardColor.Red:
                cardColor.color = colors[0];
                break;
                case CardColor.Blue:
                cardColor.color = colors[1];
                break;
                case CardColor.Black:
                cardColor.color = colors[2];
                break;
                case CardColor.Green:
                cardColor.color = colors[3];
                break;
                case CardColor.Yellow:
                cardColor.color = colors[4];
                break;
                
            }

        }
    }

    public void SetCard (CardInstance card) {
        this.card = card;
        UpdateCard();
    }

    public void Hover() {
        rectT.localPosition = new Vector3 (rectT.localPosition.x,150,0); 
    }

    public void Lower() {
        rectT.localPosition = new Vector3 (rectT.localPosition.x,0,0); 
    }

    public void StartDrag () {
        originalPos = rectT.position;
    }

    public void Drag() {
        rectT.position = Input.mousePosition;
    }

    public void Drop() {
        
        int layerMask = 1 << 6;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

        if (Physics.Raycast (ray, out hit, Mathf.Infinity, layerMask)) {
            HexagonCell cell = hit.transform.GetComponent<HexagonCell> ();

            //Play the card
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkedClient))
            {
                var player = networkedClient.PlayerObject.GetComponent<Player>();
                if (player)
                {
                    player.PlayCard (handNumber, cell.Position);
                }
            }
        } else {
            rectT.position = originalPos;
        }

    }
}
