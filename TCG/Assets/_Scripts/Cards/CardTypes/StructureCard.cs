using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Structure", menuName = "Cards/Create New Structure"), System.Serializable]
public class StructureCard : Card
{
    //Card Effects
    [SerializeField] CardEffect[] entranceEffects;
    [SerializeField] CardEffect[] turnStartEffects;
    [SerializeField] CardEffect[] turnEndEffects;

    //Keywords
    [SerializeField] StructureCardStaticKeywords staticKeywords;

    public CardEffect[] EnteranceEffects {get {return entranceEffects;}}
    public CardEffect[] TurnStartEffects {get {return turnStartEffects;}}
    public CardEffect[] TurnEndEffects   {get {return turnEndEffects;}}
}
