using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New KillUnit Spell", menuName = "Spell/Create New KillUnit Spell"), System.Serializable]
public class KillTargetSpell : Spell
{
    public override void DoEffect()
    {
        FieldUnit target = targets[0] as FieldUnit;
        target.Player.UnitToDie (target);
    }

    public override bool TragetVaildity(int targetNumber, ITargetable target, Player player)
    {
        switch (targetNumber){
            case 0: {
                if (!(target is FieldUnit)) return false;
                return true;
            }
            default:
                return false;
        }
    }



}
