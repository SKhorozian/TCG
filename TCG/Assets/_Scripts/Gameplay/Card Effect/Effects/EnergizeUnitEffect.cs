using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergizeUnitEffect : CardEffect
{
    FieldUnit unit;

    public EnergizeUnitEffect(FieldUnit unit)
    {
        this.unit = unit;
    }

    public override void DoEffect()
    {
        unit.Energize ();
    }
}
