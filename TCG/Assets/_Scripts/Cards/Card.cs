using UnityEngine;

public abstract class Card : ScriptableObject
{
    [SerializeField] string cardName;
    [SerializeField, TextArea()] string description;

    [SerializeField] CardColor color;
    
    [SerializeField] Sprite cardArt;

    [SerializeField] int cost;

    [SerializeField] CardType cardType;
    
    //Getters
    public string CardName      {get {return cardName;}}
    public string Description   {get {return description;}}
    public Sprite CardArt       {get {return cardArt;}}
    public int Cost             {get {return cost;}}
    public CardColor Color      {get {return color;}}
    public CardType Type        {get {return cardType;}}        


    public override string ToString () {
        return cardName + " | Description: " + description;
    }
}
