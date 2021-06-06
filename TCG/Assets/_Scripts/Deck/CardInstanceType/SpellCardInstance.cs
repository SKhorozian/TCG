using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCardInstance : CardInstance
{
    SpellCard spellCard;

    public SpellCardInstance (Card card) {
        this.card = card;

        if (card.GetType ().Name.Equals ("SpellCard")) {
            spellCard = (SpellCard)card;
        } else {
            throw new System.Exception ("CardType doesn't match the card's subclass");
        }
    }

    public SpellCard _SpellCard {get {return spellCard;}}
}
