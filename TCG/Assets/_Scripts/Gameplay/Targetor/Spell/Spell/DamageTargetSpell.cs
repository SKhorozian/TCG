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
        player.MatchManage.AddEffectToStack (new DamageEffect (player, damageAmount, DamageSource.Spell, Condition));
    }

    bool Condition (IDamageable damageable) {
        return damageable.Equals (targets[0]);
    }
    
    public override bool TragetVaildity(int targetNumber, ITargetable target)
    {
        switch (targetNumber) {
            case 0: {
                if (!(target is IDamageable)) {Debug.Log ("Target is not damageable"); return false;}
                if (!friendlyFire && (target as FieldCard).IsOwner) {Debug.Log ("Friendly fire is off!"); return false;}
                return true;
            }
            default:
                return false;
        }
    }
}
