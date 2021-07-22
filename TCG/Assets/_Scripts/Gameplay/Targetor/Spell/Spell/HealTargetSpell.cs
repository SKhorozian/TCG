using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New HealTarget Spell", menuName = "Spell/Create New HealTarget Spell"), System.Serializable]
public class HealTargetSpell : Spell
{
    [SerializeField] int healAmount;

    public override void DoEffect()
    {
        player.HealTarget (targets[0] as IDamageable, new Heal (healAmount, player));
    }

    public override bool TragetVaildity(int targetNumber, ITargetable target)
    {
        switch (targetNumber) {
            case 0:
                if (!(target is IDamageable)) return false;
                if ((target as FieldCard).OwnerClientId != player.OwnerClientId) return false;
            return true;
        }
        return false;
    }
}
