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
    [SerializeField] CardEffect[] entranceEffects;
    [SerializeField] CardEffect[] turnStartEffects;
    [SerializeField] CardEffect[] turnEndEffects;
    
    //Play Effect
    [SerializeField] OnPlay unitOnPlay;

    //Keywords
    [SerializeField] UnitCardStaticKeywords staticKeywords;

    public int Strength                  {get {return strength;}}
    public int Health                    {get {return health;}}
    public int MovementSpeed             {get {return movementSpeed;}}
    public int ActionPoints              {get {return actionPoints;}}
    public int AttackRange               {get {return attackRange;}}
    public CardEffect[] EntranceEffects {get {return entranceEffects;}}
    public CardEffect[] TurnStartEffects {get {return turnStartEffects;}}
    public CardEffect[] TurnEndEffects   {get {return turnEndEffects;}}
    public OnPlay UnitOnPlayEffect       {get {return unitOnPlay;}}
}
