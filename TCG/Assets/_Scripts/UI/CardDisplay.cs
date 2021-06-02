 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MLAPI;

public class CardDisplay : MonoBehaviour
{
    [Space(10),SerializeField] CardInstance card;

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
                    if (card.Card as UnitCard) {
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


    public CardInstance cardInstance {get {return card;}}
}
