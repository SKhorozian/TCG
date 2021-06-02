using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDeath : CardEffect
{
    public override void DoEffect()
    {
        if (!(fieldCard is FieldUnit)) return;     
        FieldUnit unit = fieldCard as FieldUnit;

        unit.Player.UnitDie (unit);
    }
}
