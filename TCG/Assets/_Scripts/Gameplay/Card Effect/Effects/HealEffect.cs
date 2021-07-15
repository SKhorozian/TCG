using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEffect : CardEffect
{
    int healAmount;
    Player player;
    System.Func<IDamageable, bool> condition;
    CardInstance source;

    public HealEffect(int healAmount, Player player, System.Func<IDamageable, bool> condition, CardInstance source) : base()
    {
        this.healAmount = healAmount;
        this.player = player;
        this.condition = condition;
        this.source = source;
    }

    public override void DoEffect()
    {
        foreach (FieldUnit unit in player.MatchManage.AllUnits) {
            if (condition.Invoke (unit)) {
                player.HealTarget (unit, new Heal (healAmount, player), source);
            }
        }

        if (condition.Invoke (player.FieldHero)) {
            player.HealTarget (player.FieldHero, new Heal (healAmount, player), source);
        }

        if (condition.Invoke (player.MatchManage.Player2.FieldHero)) {
            player.HealTarget (player.MatchManage.Player2.FieldHero, new Heal (healAmount, player), source);
        }
    }
}
