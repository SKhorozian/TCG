using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class UnitOnPlay : ScriptableObject
{
    [SerializeField] protected List<Target> targets;

    public abstract void OnPlay ();
}
