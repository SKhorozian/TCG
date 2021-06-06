using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroCardInstance : CardInstance
{
    HeroCard heroCard;

    public HeroCardInstance (Card card) {
        this.card = card;

        if (card is HeroCard) {
            heroCard = card as HeroCard;
        } else {
            throw new System.Exception ("CardType doesn't match the card's subclass");
        }
    }

    public HeroCard HeroCard {get {return heroCard;}}
}
