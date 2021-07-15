using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExtraCost : Targetor
{
    [SerializeField] int extraManaCost;

        public override string Location()
    {
        return "TargetorEffects/ExtraCost/" + name;
    }
}
