using UnityEngine;

public abstract class CardEffectListener : ScriptableObject
{
    [SerializeField] protected FieldCard fieldCard;

    public abstract void RegisterListener (Player player);

    public abstract void RemoveListener (Player player);

    public FieldCard FieldCard      {get {return fieldCard;} set {fieldCard = value;}}
}
