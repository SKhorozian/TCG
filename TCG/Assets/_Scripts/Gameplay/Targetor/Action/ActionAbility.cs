using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionAbility : Targetor
{
    protected FieldCard fieldCard;

    public FieldCard FieldCard {get {return fieldCard;} set {fieldCard = value;}}

    public override bool TragetVaildity (List<ITargetable> targets) {
        if (fieldCard == null) return false;

        return base.TragetVaildity (targets);
    }

    public override string Location()
    {
        return "TargetorEffects/ActionAbility/" + name;
    }
}
