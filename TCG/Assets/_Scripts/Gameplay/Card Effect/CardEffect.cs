using UnityEngine;

[System.Serializable]
public abstract class CardEffect : ScriptableObject
{
    public abstract void DoEffect (FieldCard fieldCard);
}
