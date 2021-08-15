 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MLAPI;

public class CardDisplay : MonoBehaviour
{
    CardInstance card;

    [SerializeField] Image cardBorder;
    [SerializeField] Image cardArt;

    [SerializeField] TextMeshProUGUI cardName;
    [SerializeField] TextMeshProUGUI cost;

    [SerializeField] TextMeshProUGUI description;

    [SerializeField] RectTransform textBox;
    [SerializeField] LayoutElement textBoxElement;

    [Space (10)]
    [SerializeField] Color[] colors;

    [Space(10), Header ("Unit")]
    [SerializeField] GameObject unitStuff;

    [SerializeField] TextMeshProUGUI strength;
    [SerializeField] TextMeshProUGUI health;

    [SerializeField] TextMeshProUGUI range;
    [SerializeField] TextMeshProUGUI speed;


    // Start is called before the first frame update
    void Start()
    {
        UpdateCard();
    }

    void Update () {
        textBoxElement.preferredHeight = 20 + description.preferredHeight;
    }

    void UpdateCard () {
        if (card != null) {

            cardName.text = card.CardName;

            cost.text = card.Cost.ToString();

            if (card.Description.Length > 0) {
                textBox.gameObject.SetActive (true);
                description.SetText (card.Description);
                textBoxElement.preferredHeight = 5 + description.preferredHeight;
            } else {
                textBox.gameObject.SetActive (false);
                description.SetText ("");
            }
            

            switch (card.Type) {
                case CardType.Unit:
                    if (card.Card is UnitCard) {
                        unitStuff.SetActive (true);

                        UnitCardInstance unitCard = card as UnitCardInstance;
                        strength.text = unitCard.Power.ToString();
                        health.text = unitCard.Health.ToString();   
                        range.text = unitCard.AttackRange.ToString();
                        speed.text = unitCard.MovementSpeed.ToString();
                    }
                    break;
                case CardType.Structure:
                        unitStuff.SetActive (false);

                    break;
                case CardType.Spell:
                    if (card.Card is SpellCard) {
                        unitStuff.SetActive (false);
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
                case CardColor.Pink:
                    cardBorder.color = colors[5];
                break;
                case CardColor.Colorless:
                    cardBorder.color = colors[6];
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
