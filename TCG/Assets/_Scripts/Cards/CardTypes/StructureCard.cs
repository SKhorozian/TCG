using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Structure", menuName = "Cards/Create New Structure"), System.Serializable]
public class StructureCard : Card
{
    //Card Effects
    [SerializeField] CardEffectTrigger[] cardEffects;
    //Keywords
    [SerializeField] StructureCardStaticKeywords staticKeywords;

    public CardEffectTrigger[] CardEffects {get {return cardEffects;}}

}
