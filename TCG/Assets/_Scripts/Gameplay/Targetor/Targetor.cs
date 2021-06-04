using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Targetor : ScriptableObject
{
    [SerializeField] protected TargetorPriority speed;
    [SerializeField] protected bool goesOnStack;

    protected List<ITargetable> targets;

    [SerializeField] protected int fieldTargetsCount;
    [SerializeField] protected int handTargetsCount;
    [SerializeField] protected int stackTargetsCount;

    public abstract bool TragetVaildity (List<ITargetable> targets); //Checks if targets are valid

    public void SetTargets (List<ITargetable> targets) {
        this.targets = targets;
    }

    public abstract void DoEffect ();

    public List<ITargetable> Targets {get {return targets;}}

    public TargetorPriority Speed           {get {return speed;}}
    public bool GoesOnStack              {get {return goesOnStack;}}
    public int FieldTargetsCount         {get {return fieldTargetsCount;}}
    public int HandTargetsCount          {get {return handTargetsCount;}}
    public int StackTargetsCount         {get {return stackTargetsCount;}}
}
