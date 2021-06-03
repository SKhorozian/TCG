using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureDemolition : CardEffect
{
    public override void DoEffect()
    {
        if (!(fieldCard is FieldStructure)) return;     
        FieldStructure structure = fieldCard as FieldStructure;

        structure.Player.StructureDemolish (structure);
    }
}
