using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New KillUnit Spell", menuName = "Spell/Create New KillUnit Spell"), System.Serializable]
public class KillTargetSpell : Spell
{
    [SerializeField] bool friendlyFire;

    public override void DoEffect()
    {
        FieldUnit target = targets[0] as FieldUnit;
        target.Player.UnitToDie (target);
    }

    public override bool TragetVaildity(List<ITargetable> targets)
    {
        if (targets.Count != 1) return false;
        if (targets[0] == null) return false;
        if (!(targets[0] is FieldUnit)) return false;

        if (!friendlyFire && (targets[0] as FieldUnit).OwnerClientId.Equals (player.OwnerClientId)) return false;

        return true;
    }
}
