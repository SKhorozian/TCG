using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Targetor : ScriptableObject
{
    [SerializeField] protected TargetorPriority speed;

    protected List<ITargetable> targets;

    protected Player player;

    [SerializeField] protected TargetType[] targetTypes;

    [SerializeField, TextArea (minLines: 4, maxLines: 8)] string description;

    public virtual bool TragetVaildity (List<ITargetable> targets) {
        if (targets.Count != targetTypes.Length) {Debug.Log ("Target length doesn't match"); return false;}

        foreach (ITargetable target in targets) {
            if (target == null) {Debug.Log ("Target is null"); return false;}
            if (!TragetVaildity (targets.IndexOf (target), target)) {Debug.Log ("Target " + targets.IndexOf (target) + " is invalid"); return false;}; 
        } 

        return true;
    }
    public abstract bool TragetVaildity (int targetNumber, ITargetable target); //Checks if target is valid

    public void SetTargets (List<ITargetable> targets) {
        this.targets = targets;
    }

    public abstract string Location ();

    public abstract void DoEffect ();

    public List<ITargetable> Targets {get {return targets;}}

    public TargetorPriority Speed        {get {return speed;}}

    public TargetType[] TargetTypes      {get {return targetTypes;}}

    public Player Player                 {get {return player;} set {player = value;}}
    public string Description            {get {return description;}}
}