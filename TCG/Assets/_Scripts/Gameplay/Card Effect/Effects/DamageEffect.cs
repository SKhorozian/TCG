using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : CardEffect
{
    int damageAmount;
    DamageSource source;
    System.Func<IDamageable, bool> condition;


    public DamageEffect(FieldCard fieldCard, int damageAmount, DamageSource source, System.Func<IDamageable, bool> condition) : base(fieldCard)
    {
        this.damageAmount = damageAmount;
        this.source = source;
        this.condition = condition;
    }

    public override void DoEffect()
    {
        foreach (FieldUnit unit in fieldCard.Player.MatchManage.AllUnits) {
            if (condition.Invoke (unit)) {
                fieldCard.Player.DamageTarget (unit, new Damage (damageAmount, source, fieldCard.Player));
            }
        }

        if (condition.Invoke (fieldCard.Player.FieldHero)) {
            fieldCard.Player.DamageTarget (fieldCard.Player.FieldHero, new Damage (damageAmount, source, fieldCard.Player));
        }

        if (condition.Invoke (fieldCard.Player.MatchManage.Player2.FieldHero)) {
            fieldCard.Player.DamageTarget (fieldCard.Player.MatchManage.Player2.FieldHero, new Damage (damageAmount, source, fieldCard.Player));
        }
    }
}
