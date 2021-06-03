using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureCardInstance : CardInstance
{
    protected StructureCard structureCard;

    public StructureCardInstance (Card card) {
        this.card = card;

        if (card is StructureCard) {
            structureCard = card as StructureCard;
        } else {
            throw new System.Exception ("CardType doesn't match the card's subclass");
        }
    }

    public StructureCard _StructureCard    {get {return structureCard;}}

}
