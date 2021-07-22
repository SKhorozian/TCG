using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New BuffUnitEnergize Spell", menuName = "Spell/Create New BuffUnitEnergize Spell"), System.Serializable]
public class BuffUnitAndEnergizeSpell : BuffUnitSpell
{
    public override void DoEffect()
    {
        base.DoEffect ();
        (targets[0] as FieldUnit).Energize ();
    }
}
