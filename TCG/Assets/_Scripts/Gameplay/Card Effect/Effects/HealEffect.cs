using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEffect : CardEffect
{
    int healAmount;
    System.Func<IDamageable, bool> condition;

    public HealEffect(FieldCard fieldCard, int healAmount, System.Func<IDamageable, bool> condition) : base(fieldCard)
    {
        this.healAmount = healAmount;
        this.condition = condition;
    }

    public override void DoEffect()
    {
        foreach (FieldUnit unit in fieldCard.Player.MatchManage.AllUnits) {
            if (condition.Invoke (unit)) {
                fieldCard.Player.HealTarget (unit, new Heal (healAmount, fieldCard.Player), fieldCard);
            }
        }

        if (condition.Invoke (fieldCard.Player.FieldHero)) {
            fieldCard.Player.HealTarget (fieldCard.Player.FieldHero, new Heal (healAmount, fieldCard.Player), fieldCard);
        }

        if (condition.Invoke (fieldCard.Player.MatchManage.Player2.FieldHero)) {
            fieldCard.Player.HealTarget (fieldCard.Player.MatchManage.Player2.FieldHero, new Heal (healAmount, fieldCard.Player), fieldCard);
        }
    }
}
