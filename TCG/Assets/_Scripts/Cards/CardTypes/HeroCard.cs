using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Hero", menuName = "Cards/Create New Hero"), System.Serializable]
public class HeroCard : Card
{
    //Card Effects
    [SerializeField] CardEffect[] gameStartEffects;
    [SerializeField] CardEffect[] turnStartEffects;
    [SerializeField] CardEffect[] turnEndEffects;

    //Actions
    [SerializeField] List<Action> actions;

    public CardEffect[] GameStartEffects {get {return gameStartEffects;}}
    public CardEffect[] TurnStartEffects {get {return turnStartEffects;}}
    public CardEffect[] TurnEndEffects   {get {return turnEndEffects;}}
    public List<Action> Actions          {get {return actions;}}


}
