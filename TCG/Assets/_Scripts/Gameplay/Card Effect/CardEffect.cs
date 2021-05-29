using UnityEngine;

[System.Serializable]
public abstract class CardEffect : ScriptableObject
{
    protected FieldCard fieldCard;

    public abstract void DoEffect ();
    public FieldCard FieldCard {get {return fieldCard;} set {fieldCard = value;}}
}
