using UnityEngine;

[CreateAssetMenu (fileName = "New Card", menuName = "Cards/Create New Card")]
[System.Serializable]
public class Card : ScriptableObject
{
    [SerializeField] string cardName;
    [SerializeField, TextArea()] string description;

    [SerializeField] CardColor color;
    
    [SerializeField] Sprite cardArt;

    //Stats
    [SerializeField] int cost;
    [SerializeField] int strength;
    [SerializeField] int health;

    //Getters
    public string CardName      {get {return cardName;}}
    public string Description   {get {return description;}}
    public Sprite CardArt       {get {return cardArt;}}
    public int Cost             {get {return cost;}}
    public int Strength         {get {return strength;}}
    public int Health           {get {return health;}}
    public CardColor Color      {get {return color;}}

    public override string ToString () {
        return cardName + " | Description: " + description;
    }
}
