using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "TreeEffect", menuName = "Card Effects/Trigger/Green/Structure/TreeEffect")]
public class TreeEffect : CardEffectTrigger
{
    public override CardEffect GetCardEffect()
    {
        return new HealEffect (fieldCard, 100, Condition);
    }

    bool Condition (IDamageable damageable) {
        if (damageable is FieldUnit) {
            FieldUnit unit = damageable as FieldUnit;
            return fieldCard.Player.FieldUnits.Contains (unit);
        } else if (damageable is FieldHero) {
            return (damageable as FieldHero).Equals (fieldCard.Player.FieldHero);
        }   

        return false;
    }
}
