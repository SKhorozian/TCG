using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Hero", menuName = "Cards/Create New Hero"), System.Serializable]
public class HeroCard : Card
{
    //Card Effects
    [SerializeField] CardEffectTrigger[] cardEffects;

    //Actions
    [SerializeField] List<ActionAbility> actions;

    public CardEffectTrigger[] CardEffects {get {return cardEffects;}}

}
