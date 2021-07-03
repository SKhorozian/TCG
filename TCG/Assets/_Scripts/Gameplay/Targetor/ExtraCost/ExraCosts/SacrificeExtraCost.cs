using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Sacrifice ExtraCost", menuName = "ExtraCost/Create new Sacrifice ExtraCost")]
public class SacrificeExtraCost : ExtraCost
{
    public override void DoEffect()
    {
        if (targets[0] is FieldUnit) {
            FieldUnit fieldUnit = targets[0] as FieldUnit;
            fieldUnit.Player.UnitToDie (fieldUnit);
        } else if (targets[0] is FieldStructure) {
            FieldStructure fieldStructure = targets[0] as FieldStructure;
            fieldStructure.Player.StructureToDemolish (fieldStructure);
        }
    }

    public override bool TragetVaildity(int targetNumber, ITargetable target)
    {
        switch (targetNumber) {
            case 0: {
                if (!(target is FieldCard)) return false;
                if (!(target as FieldCard).OwnerClientId.Equals (player.OwnerClientId)) return false;

                return true;
            }
            default:
                return false;
        }
    }
}