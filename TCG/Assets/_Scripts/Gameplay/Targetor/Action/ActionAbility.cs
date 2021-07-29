using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionAbility : Targetor
{
    protected FieldCard fieldCard;

    [SerializeField] int manaCost;
    [SerializeField] bool usesActionPoint;
    [SerializeField] ExtraCost extraCost;

    public FieldCard FieldCard  {get {return fieldCard;} set {fieldCard = value;}}
    public int ManaCost         {get {return manaCost;}}
    public bool UsesActionPoint {get {return usesActionPoint;}}
    public ExtraCost ExtraCost  {get {return extraCost;}}

    public override bool TragetVaildity (List<ITargetable> targets) {
        if (fieldCard == null) return false;

        return base.TragetVaildity (targets);
    }

    public override string Location()
    {
        return "TargetorEffects/ActionAbility/" + name;
    }
}
