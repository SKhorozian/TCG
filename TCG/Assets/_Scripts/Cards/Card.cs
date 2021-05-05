using UnityEngine;

[CreateAssetMenu (fileName = "New Card", menuName = "Cards/Create New Card")]
[System.Serializable]
public class Card : ScriptableObject
{
    [SerializeField] string cardName;

    public string CardName {
        get {
            return cardName;
        }
    }
}

