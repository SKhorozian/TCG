using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Unit", menuName = "Cards/Create New Unit"), System.Serializable]
public class UnitCard : Card
{
    [Space (20)]
    [SerializeField] int strength;
    [SerializeField] int health;
    [SerializeField] int actionPoints;
    [SerializeField] int movementSpeed;
    [SerializeField] int attackRange;

    [Space (20)]
    //Card Effects
    [SerializeField] CardEffectTrigger[] cardEffects;

    [Space (20)]
    //Keywords
    [SerializeField] UnitCardStaticKeywords staticKeywords;

    [Space (20)]
    //Play Effect
    [SerializeField] OnPlay onPlayEffect;

    [Space (20)]
    //Actions
    [SerializeField] List<Action> actions;
    

    public int Strength                             {get {return strength;}}
    public int Health                               {get {return health;}}
    public int MovementSpeed                        {get {return movementSpeed;}}
    public int ActionPoints                         {get {return actionPoints;}}
    public int AttackRange                          {get {return attackRange;}}
    public UnitCardStaticKeywords StaticKeywords    {get {return staticKeywords;}}
    public CardEffectTrigger[] CardEffects          {get {return cardEffects;}}
    public OnPlay OnPlayEffect                      {get {return onPlayEffect;}}
    public List<Action> Actions                     {get {return actions;}}
}
