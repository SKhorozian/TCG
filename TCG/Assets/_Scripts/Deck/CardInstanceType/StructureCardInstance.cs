using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureCardInstance : CardInstance
{
    protected StructureCard structureCard;

    //Stats
    int bonusHealth;

    public StructureCardInstance (Card card) {
        this.card = card;

        if (card is StructureCard) {
            structureCard = card as StructureCard;
        } else {
            throw new System.Exception ("CardType doesn't match the card's subclass");
        }

        this.bonusHealth = 0;
    }

    public StructureCardInstance (Card card, int health) {
        this.card = card;

        if (card is StructureCard) {
            structureCard = card as StructureCard;
        } else {
            throw new System.Exception ("CardType doesn't match the card's subclass");
        }

        this.bonusHealth = health;
    }

    public int Health                     {get {return structureCard.Health + bonusHealth;}}
    public StructureCard StructureCard    {get {return structureCard;}}

}
