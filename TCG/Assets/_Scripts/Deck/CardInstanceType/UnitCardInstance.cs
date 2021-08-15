using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCardInstance : CardInstance
{
    protected UnitCard unitCard;

    //Card Stats
    int powerBonus; 
    int healthBonus; 
    int rangeBonus;
    int speedBonus;

    public UnitCardInstance (Card card) {
        this.card = card;

        if (card is UnitCard) {
            unitCard = card as UnitCard;
        } else {
            throw new System.Exception ("CardType doesn't match the card's subclass");
        }

        this.powerBonus = 0;
        this.healthBonus = 0;
        this.speedBonus = 0;
        this.rangeBonus = 0;
        this.costChange = 0;
    }

    public void GiveStats (int power, int health, int range, int speed) {
        powerBonus += power;
        healthBonus += health;
        rangeBonus += range;
        speedBonus += speed;
    }


    public UnitCardInstance (Card card, CardInstanceInfo cardInfo) {
        this.card = card;

        if (card is UnitCard) {
            unitCard = card as UnitCard;
        } else {
            throw new System.Exception ("CardType doesn't match the card's subclass");
        }

        this.powerBonus = cardInfo.bonusPower;
        this.healthBonus = cardInfo.bonusHealth;
        this.speedBonus = cardInfo.bonusSpeed;
        this.rangeBonus = cardInfo.bonusRange;
        this.costChange = cardInfo.costChange;
    }

    public void SetCardInfo (CardInstanceInfo cardInfo) {
        this.powerBonus = cardInfo.bonusPower;
        this.healthBonus = cardInfo.bonusHealth;
        this.speedBonus = cardInfo.bonusSpeed;
        this.rangeBonus = cardInfo.bonusRange;
        this.costChange = cardInfo.costChange;
    }

    public UnitCard UnitCard    {get {return unitCard;}}

    public int Power         {get {return UnitCard.Power + powerBonus;}}
    public int Health           {get {return UnitCard.Health + healthBonus;}}
    public int MovementSpeed    {get {return UnitCard.MovementSpeed + speedBonus;}}
    public int AttackRange      {get {return UnitCard.AttackRange + rangeBonus;}}

    public int PowerBonus    {get {return powerBonus;}}
    public int HealthBonus      {get {return healthBonus;}}
    public int RangeBonus       {get {return rangeBonus;}}
    public int SpeedBonus       {get {return speedBonus;}}
}
