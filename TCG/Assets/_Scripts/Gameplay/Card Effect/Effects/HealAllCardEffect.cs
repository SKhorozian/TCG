using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New HealAll Effect", menuName = "Card Effects/Create New HealAll Effect"), System.Serializable]
public class HealAllCardEffect : CardEffect
{
    [SerializeField] int healAmount;

    public override void DoEffect()
    {
        foreach (FieldUnit unit in fieldCard.Player.FieldUnits) {
            unit.health.Value += healAmount;
        }
    }
}
