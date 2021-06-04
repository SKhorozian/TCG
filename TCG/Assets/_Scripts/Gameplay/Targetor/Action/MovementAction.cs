using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAction : Action
{
    public override void DoEffect()
    {
        
    }

    public override bool TragetVaildity(List<ITargetable> targets)
    {
        return false;
    }
}
