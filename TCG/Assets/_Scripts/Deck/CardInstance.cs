using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI.Serialization;

[System.Serializable]
public abstract class CardInstance : ITargetable
{
    [SerializeField] protected Card card;
    
    //Card Stats
    protected int costChange;

    //Change the card 
    public void ChangeCard (Card newCard) {
        card = newCard;
    }
    
    //Keywords 

    //Status Stuff
    protected List<StatusEffect> effects = new List<StatusEffect> ();

    public void AddStatusEffect (StatusEffect effect) {
        effects.Add (effect);
        effect.ApplyStatus (this);
    }

    public void RemoveStatusEffect (StatusEffect effect) {
        if (effects.Contains (effect)) {
            effect.RemoveStatus (this);
        }
    }

    public Card Card            {get {return card;}}
    public int Cost             {get {return card.Cost + costChange;}}
    public int CostChange       {get {return costChange;}}

    public string CardName      {get {return card.CardName;}}
    public string Description   {get {return card.Description;}}
    public Sprite CardArt       {get {return card.CardArt;}}
    public CardColor Color      {get {return card.Color;}}
    public CardType Type        {get {return card.Type;}}
    public string CardLocation  {get {return "Cards/" + Color.ToString() + "/" + card.name;}}
    public List<StatusEffect> StatusEffects     {get {return effects;}}
}
