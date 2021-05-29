using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New ManaRefill Effect", menuName = "Card Effects/Create New ManaRefill Effect"), System.Serializable]
public class ManaRefillEffect : CardEffect
{
    [SerializeField] int refillAmount;

    public override void DoEffect()
    {
        fieldCard.Player.RefillMana (refillAmount);
    }
}
