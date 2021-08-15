using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "TreeEffect", menuName = "Card Effects/Trigger/Green/Structure/TreeEffect")]
public class TreeEffect : CardEffectTrigger
{
    public override CardEffect GetCardEffect()
    {
        return new HealEffect (100, fieldCard.Player, Condition, fieldCard.Card);
    }

    bool Condition (IDamageable damageable) {
        if (damageable.Equals (fieldCard)) return false;

        if (damageable is FieldCard) {
            FieldCard card = damageable as FieldCard;
            return card.Player.Equals (fieldCard.Player);
        }

        return false;
    }
}
