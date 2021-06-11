using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New DamageTarget Spell", menuName = "Spell/Create New DamageTarget Spell"), System.Serializable]
public class DamageTargetSpell : Spell
{
    [SerializeField] int damageAmount;
    [SerializeField] bool friendlyFire;

    public override void DoEffect()
    {
        IDamageable target = targets[0] as IDamageable;
        player.DamageTarget (target, new Damage (damageAmount, DamageSource.Spell, player), spellCardInstance);
    }

    public override bool TragetVaildity(List<ITargetable> targets)
    {
        if (targets.Count != 1) return false;
        if (targets[0] == null) return false;
        if (!(targets[0] is IDamageable)) return false;

        if (!friendlyFire && (targets[0] as FieldUnit).OwnerClientId.Equals (player.OwnerClientId)) return false;

        return true;
    }
}
