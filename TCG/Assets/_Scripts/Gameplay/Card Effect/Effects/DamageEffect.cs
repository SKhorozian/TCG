using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : CardEffect
{
    Player player;
    int damageAmount;
    DamageSource source;
    System.Func<IDamageable, bool> condition;


    public DamageEffect(Player player, int damageAmount, DamageSource source, System.Func<IDamageable, bool> condition) : base()
    {
        this.player = player;
        this.damageAmount = damageAmount;
        this.source = source;
        this.condition = condition;
    }

    public override void DoEffect()
    {
        foreach (FieldUnit unit in player.MatchManage.AllUnits) {
            if (condition.Invoke (unit)) {
                player.DamageTarget (unit, new Damage (damageAmount, source, player));
            }
        }

        if (condition.Invoke (player.FieldHero)) {
            player.DamageTarget (player.FieldHero, new Damage (damageAmount, source, player));
        }

        if (condition.Invoke (player.MatchManage.Player2.FieldHero)) {
            player.DamageTarget (player.MatchManage.Player2.FieldHero, new Damage (damageAmount, source, player));
        }
    }
}
