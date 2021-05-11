using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MLAPI;

public class CardDisplay : MonoBehaviour
{
    [SerializeField] Card card;
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

            strength.text = card.Strength.ToString();
            health.text = card.Health.ToString();

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

    public void SetCard (Card card) {
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
        if (Input.mousePosition.y > Screen.height)
            rectT.position = originalPos;
        else {
            //Play the card
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkedClient))
            {
                var player = networkedClient.PlayerObject.GetComponent<Player>();
                if (player)
                {
                    player.PlayCard (handNumber);
                }
            }
        }
    }
}
