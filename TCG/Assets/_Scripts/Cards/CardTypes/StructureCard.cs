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
    [SerializeField] OnPlay onPlayEffect;

    [Space (20)]
    //Keywords
    [SerializeField] StructureCardStaticKeywords staticKeywords;

    public CardEffectTrigger[] CardEffects {get {return cardEffects;}}
    public OnPlay OnPlayEffect  {get {return onPlayEffect;}}

}
