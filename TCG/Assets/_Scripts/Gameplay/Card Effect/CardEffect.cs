using UnityEngine;

public abstract class CardEffect
{
    protected FieldCard fieldCard;

    public CardEffect (FieldCard fieldCard) {
        this.fieldCard = fieldCard;
    }

    public abstract void DoEffect ();
    public FieldCard FieldCard {get {return fieldCard;}}
}
