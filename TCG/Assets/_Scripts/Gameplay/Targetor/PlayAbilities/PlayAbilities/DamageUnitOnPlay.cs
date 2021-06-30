using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New DamageUnit OnPlay", menuName = "Card OnPlay/Create New DamageUnit OnPlay"), System.Serializable]
public class DamageUnitOnPlay : PlayAbility
{
    [SerializeField] int damage;
    [SerializeField] bool friendlyFire;

    public override void DoEffect()
    {
        FieldUnit target = targets[0] as FieldUnit;

        playCard.Player.DamageTarget (target, new Damage (damage, DamageSource.Spell, playCard.Player));
    }

    public override bool TragetVaildity(int targetNumber, ITargetable target, Player player)
    {
        switch (targetNumber) {
            case 0: {
                if (!(target is FieldUnit)) return false;
                // if (playCard) //On Server
                //     if ((target as FieldUnit).Player.Equals (playCard.Player)) return false;
                // else //Client Side
                //     if ((target as FieldUnit).IsOwner) return false;
                return true;
            }
            default:
                return false;
        }
    }
}
