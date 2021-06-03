 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MLAPI;

public class CardDisplay : MonoBehaviour
{
    [Space(10),SerializeField] CardInstance card;

    [SerializeField] Image cardBorder;
    [SerializeField] Image cardArt;

    [SerializeField] TextMeshProUGUI cardName;
    [SerializeField] TextMeshProUGUI cost;

    [Header ("Unit"), Space (10)]
    [SerializeField] GameObject unitStuff;
    [SerializeField] TextMeshProUGUI unitDescription;
    [SerializeField] TextMeshProUGUI strength;
    [SerializeField] TextMeshProUGUI health;
    [SerializeField] TextMeshProUGUI range;
    [SerializeField] TextMeshProUGUI movementSpeed;

    [Space (10), Header ("Structure")]
    [SerializeField] GameObject structStuff;
    [SerializeField] TextMeshProUGUI structDescription;

    [SerializeField] Color[] colors;

    [Space (10)]
    [SerializeField] Sprite unitCardBorder;
    [SerializeField] Sprite structureCardBorder;
    [SerializeField] Sprite spellCardBorder;


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

            cost.text = card.Cost.ToString();

            switch (card.Type) {
                case CardType.Unit:

                    if (card.Card is UnitCard) {
                        cardBorder.sprite = unitCardBorder;

                        unitStuff.SetActive (true);
                        structStuff.SetActive (false);

                        UnitCardInstance unitCard = card as UnitCardInstance;
                        strength.text = unitCard.Strength.ToString();
                        health.text = unitCard.Health.ToString();   
                        range.text = unitCard.AttackRange.ToString ();
                        movementSpeed.text = unitCard.MovementSpeed.ToString ();
                        unitDescription.text = card.Description;
                    }

                    break;
                case CardType.Structure:

                    if (card.Card is StructureCard) {
                        cardBorder.sprite = structureCardBorder;

                        unitStuff.SetActive (false);
                        structStuff.SetActive (true);
                        structDescription.text = card.Description;
                    }

                    break;
                default:
                break;
            }


            cardArt.sprite = card.CardArt;

            switch (card.Color) {
                case CardColor.Red:
                cardBorder.color = colors[0];
                break;
                case CardColor.Blue:
                cardBorder.color = colors[1];
                break;
                case CardColor.Black:
                cardBorder.color = colors[2];
                break;
                case CardColor.Green:
                cardBorder.color = colors[3];
                break;
                case CardColor.Yellow:
                cardBorder.color = colors[4];
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
