using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScavengerEffect : CardEffect
{
    MatchManager matchManager;

    public ScavengerEffect (MatchManager matchManager) {
        this.matchManager = matchManager;
    }

    public override void DoEffect()
    {
        foreach (FieldUnit unit in matchManager.AllUnits) {
            if (unit.Card.Card.StaticKeywords.Contains (StaticKeyword.Scavenger)) {
                StatsStatusEffect statusEffect = new StatsStatusEffect (StatusDuration.Permanent, 50, 0, 0, 0);
                ApplyFieldCardStatusEffect effect = new ApplyFieldCardStatusEffect (statusEffect, unit);
                effect.DoEffect ();
            }
        }
    }
}
