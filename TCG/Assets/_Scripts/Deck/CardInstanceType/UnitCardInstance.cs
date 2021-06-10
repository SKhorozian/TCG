using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCardInstance : CardInstance
{
    protected UnitCard unitCard;

    //Card Stats
    int strengthBonus; 
    int healthBonus; 

    public UnitCardInstance (Card card) {
        this.card = card;

        if (card is UnitCard) {
            unitCard = card as UnitCard;
        } else {
            throw new System.Exception ("CardType doesn't match the card's subclass");
        }

        this.strengthBonus = 0;
        this.healthBonus = 0;
        this.costChange = 0;
    }

    public UnitCard UnitCard    {get {return unitCard;}}

    public int Strength         {get {return UnitCard.Strength + strengthBonus;}}
    public int Health           {get {return UnitCard.Health + healthBonus;}}
    public int ActionPoints     {get {return UnitCard.ActionPoints;}}
    public int MovementSpeed    {get {return UnitCard.MovementSpeed;}}
    public int AttackRange      {get {return UnitCard.AttackRange;}}

    public int StrengthBonus    {get {return strengthBonus;}}
    public int HealthBonus      {get {return healthBonus;}}
}
