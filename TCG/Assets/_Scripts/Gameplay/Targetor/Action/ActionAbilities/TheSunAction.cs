using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New TheSunAction Action", menuName = "Action/Create New TheSunAction Action"), System.Serializable]
public class TheSunAction : ActionAbility
{
    [SerializeField] int tallyAmount;

    public override void DoEffect()
    {
        fieldCard.Tally (tallyAmount);
    }

    public override bool TragetVaildity (List<ITargetable> targets) {
        if (fieldCard.tallies.Value >= 4) return false;
        return base.TragetVaildity (targets);
    }

    public override bool TragetVaildity(int targetNumber, ITargetable target)
    {
        return true;
    }
}
