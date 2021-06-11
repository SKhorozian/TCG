using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "TreantEntranceEffect", menuName = "Card Effects/Trigger/Green/Unit/TreantEntranceEffect")]
public class TreantEntranceEffect : CardEffectTrigger
{
    public override CardEffect GetCardEffect()
    {
        return new HealEffect (fieldCard, 200, Condition);
    }

    bool Condition (IDamageable damageable) {
        return damageable.Equals (fieldCard.Player.FieldHero);
    }
}
