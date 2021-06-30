using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardEffectTrigger : ScriptableObject
{   
    [SerializeField] EffectTrigger trigger;
    [SerializeField] protected FieldCard fieldCard;

    public abstract CardEffect GetCardEffect ();

    public EffectTrigger Trigger    {get {return trigger;}}
    public FieldCard FieldCard      {get {return fieldCard;} set {fieldCard = value;}}
}

[System.Flags]
public enum EffectTrigger {
    Entrance = (1 << 0),       //When it enters the field
    TurnStart = (1 << 1),      //At turn start
    TurnEnd = (1 << 2),        //At turn end
    Movement = (1 << 3),       //When choosing the movement action
    Move = (1 << 4),           //When moving by any means
    Strike = (1 << 5),         //When striking another unit
    Attack = (1 << 6),         //When choosing the attack action
    Sacrificed = (1 << 7),     //When sacrificed
    OnDestroy = (1 << 8),      //When it dies or is demolished
    DamageTaken = (1 << 9),    //When it takes damage
    Aura = (1 << 10),           
    Usurp = (1 << 11)          //When striking the enemy hero
}