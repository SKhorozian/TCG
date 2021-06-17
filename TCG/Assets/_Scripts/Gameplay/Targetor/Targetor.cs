using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Targetor : ScriptableObject
{
    [SerializeField] protected TargetorPriority speed;
    [SerializeField] protected bool goesOnStack;

    protected List<ITargetable> targets;

    [SerializeField] protected TargetType[] targetTypes;

    [SerializeField, TextArea (minLines: 4, maxLines: 8)] string description;

    public bool TragetVaildity (List<ITargetable> targets, Player player) {
        if (targets.Count != targetTypes.Length) {Debug.Log ("Target length doesn't match"); return false;}

        foreach (ITargetable target in targets) {
            if (target == null) {Debug.Log ("Target is null"); return false;}
            if (!TragetVaildity (targets.IndexOf (target), target, player)) {Debug.Log ("Target " + targets.IndexOf (target) + " is invalid"); return false;}; 
        } 

        return true;
    }
    public abstract bool TragetVaildity (int targetNumber, ITargetable target, Player player); //Checks if target is valid

    public void SetTargets (List<ITargetable> targets) {
        this.targets = targets;
    }

    public abstract void DoEffect ();

    public List<ITargetable> Targets {get {return targets;}}

    public TargetorPriority Speed        {get {return speed;}}
    public bool GoesOnStack              {get {return goesOnStack;}}

    public TargetType[] TargetTypes      {get {return targetTypes;}}

    public string Description            {get {return description;}}
}