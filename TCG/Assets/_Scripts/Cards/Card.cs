using UnityEngine;

public abstract class Card : ScriptableObject
{
    [SerializeField] string cardName;

    [SerializeField] int cost;

    [SerializeField, TextArea(minLines: 10, maxLines: 20)] string description;

    [SerializeField] CardColor color;
    
    [SerializeField] Sprite cardArt;

    [SerializeField] CardType cardType;
    
    [Space (20)]
    //Play Effect
    [SerializeField] OnPlay onPlayEffect;

    //Getters
    public string CardName      {get {return cardName;}}
    public string Description   {get {return description;}}
    public Sprite CardArt       {get {return cardArt;}}
    public int Cost             {get {return cost;}}
    public CardColor Color      {get {return color;}}
    public CardType Type        {get {return cardType;}}        
    public OnPlay OnPlayEffect  {get {return onPlayEffect;}}


    public override string ToString () {
        return cardName + " | Description: " + description;
    }
}
