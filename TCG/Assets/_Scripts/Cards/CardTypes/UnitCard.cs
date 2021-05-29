using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Unit", menuName = "Cards/Create New Unit"), System.Serializable]
public class UnitCard : Card
{
    [SerializeField] int strength;
    [SerializeField] int health;
    [SerializeField] int actionPoints;
    [SerializeField] int movementSpeed;
    [SerializeField] int attackRange;

    //Card Effects
    [SerializeField] CardEffect[] enteranceEffects;
    [SerializeField] CardEffect[] turnStartEffects;
    [SerializeField] CardEffect[] turnEndEffects;
    
    //Play Effect
    [SerializeField] OnPlay unitOnPlay;

    //Keywords
    [SerializeField] CardKeywords keywords;

    public int Strength                  {get {return strength;}}
    public int Health                    {get {return health;}}
    public int MovementSpeed             {get {return movementSpeed;}}
    public int ActionPoints              {get {return actionPoints;}}
    public int AttackRange               {get {return attackRange;}}
    public CardEffect[] EnteranceEffects {get {return enteranceEffects;}}
    public CardEffect[] TurnStartEffects {get {return turnStartEffects;}}
    public CardEffect[] TurnEndEffects   {get {return turnEndEffects;}}
}
