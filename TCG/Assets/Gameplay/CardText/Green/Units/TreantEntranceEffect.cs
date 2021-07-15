using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "TreantEntranceEffect", menuName = "Card Effects/Trigger/Green/Unit/TreantEntranceEffect")]
public class TreantEntranceEffect : CardEffectTrigger
{
    public override CardEffect GetCardEffect()
    {
        return new HealEffect (200, fieldCard.Player, Condition, fieldCard.Card);
    }

    bool Condition (IDamageable damageable) {
        return damageable.Equals (fieldCard.Player.FieldHero);
    }
}
