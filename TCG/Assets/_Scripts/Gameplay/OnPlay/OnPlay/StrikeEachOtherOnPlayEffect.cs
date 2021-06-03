using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New StrikeEachOther OnPlay", menuName = "Card OnPlay/Create New StrikeEachOther OnPlay"), System.Serializable]
public class StrikeEachOtherOnPlayEffect : OnPlay
{
    [SerializeField] bool friendlyFire;

    public override void PlayEffect()
    {
        if (targets[0] is FieldUnit) {
            playUnit.Strike (targets[0] as FieldUnit);
            (targets[0] as FieldUnit).Strike (playUnit);
        }
    }

    public override bool TragetVaildity(List<ITargetable> targets)
    {
        if (targets.Count != 1) return false;
        if (targets[0] == null) return false;
        if (!(targets[0] is FieldUnit)) return false;

        if (!friendlyFire && (targets[0] as FieldUnit).OwnerClientId.Equals (playUnit.OwnerClientId)) return false;

        return true;
    }
}
