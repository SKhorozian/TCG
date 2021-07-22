using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

[CreateAssetMenu (fileName = "New DamageUnit OnPlay", menuName = "Card OnPlay/Create New DamageUnit OnPlay"), System.Serializable]
public class DamageUnitOnPlay : PlayAbility
{
    [SerializeField] int damage;
    [SerializeField] bool friendlyFire;

    public override void DoEffect()
    {
        FieldUnit target = targets[0] as FieldUnit;

        player.DamageTarget (target, new Damage (damage, DamageSource.Spell, player));
    }

    public override bool TragetVaildity(int targetNumber, ITargetable target)
    {
        switch (targetNumber) {
            case 0: {
                if (!(target is FieldUnit)) return false;
                if ((target as FieldUnit).Player.OwnerClientId.Equals (player.OwnerClientId)) return false;
                return true;
            }
            default:
                return false;
        }
    }
}
