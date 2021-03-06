using UnityEngine;
using System.Collections.Generic;

public abstract class Card : ScriptableObject
{
    [SerializeField] string cardName;

    [SerializeField] int cost;

    [SerializeField, TextArea(minLines: 10, maxLines: 20)] string description;

    [SerializeField] CardColor color;
    
    [SerializeField] Sprite cardArt;

    [SerializeField] CardType cardType;

    [SerializeField] CardTribe cardTribe;

    [SerializeField] CardRarity cardRarity;

    [Space (20)]
    //Keywords
    [SerializeField] List<StaticKeyword> staticKeywords;

    [Space (10), SerializeField] ExtraCost extraCost;

    //Getters
    public string CardName      {get {return cardName;}}
    public string Description   {get {return description;}}
    public Sprite CardArt       {get {return cardArt;}}
    public int Cost             {get {return cost;}}
    public CardColor Color      {get {return color;}}
    public CardType Type        {get {return cardType;}}   
    public CardTribe Tribe      {get {return cardTribe;}}
    public HashSet<StaticKeyword> StaticKeywords    {get {return new HashSet<StaticKeyword> (staticKeywords);}}
    public ExtraCost ExtraCost  {get {return extraCost;}}

    public override string ToString () {
        return cardName + " | Description: " + description;
    }
}
