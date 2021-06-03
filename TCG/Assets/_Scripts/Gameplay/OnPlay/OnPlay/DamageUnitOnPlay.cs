using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New DamageUnit OnPlay", menuName = "Card OnPlay/Create New DamageUnit OnPlay"), System.Serializable]
public class DamageUnitOnPlay : OnPlay
{
    [SerializeField] int damage;
    [SerializeField] bool friendlyFire;

    public override void PlayEffect()
    {
        if (targets[0] is FieldUnit) {
            FieldUnit target = targets[0] as FieldUnit;
            playUnit.Player.DamageTarget (target, new Damage (damage, DamageSource.SingleTargetSpell, playUnit.Player));
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
