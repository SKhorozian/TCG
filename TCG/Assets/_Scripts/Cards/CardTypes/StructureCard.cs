using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Structure", menuName = "Cards/Create New Structure"), System.Serializable]
public class StructureCard : Card
{
    [Space (20)]
    //Card Effects
    [SerializeField] CardEffectTrigger[] cardEffects;

    [Space (20)]
    //Play Effect
    [SerializeField] PlayAbility onPlayEffect;

    [Space (20)]
    //Actions
    [SerializeField] List<ActionAbility> actions;

    [Space (20)]
    //Keywords
    [SerializeField] StructureCardStaticKeywords staticKeywords;

    public CardEffectTrigger[] CardEffects {get {return cardEffects;}}
    public PlayAbility OnPlayEffect  {get {return onPlayEffect;}}
    public List<ActionAbility> Actions  {get {return actions;}}

}
