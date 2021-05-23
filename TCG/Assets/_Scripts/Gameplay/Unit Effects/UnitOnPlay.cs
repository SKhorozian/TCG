using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class UnitOnPlay : ScriptableObject
{
    [SerializeField] protected List<Target> targets;
    [SerializeField] protected bool isOptional;

    protected List<CardInstance> _handCards;
    protected List<FieldUnit> _fieldUnits;
    protected Player player;

    public bool SetTarget (List<CardInstance> handCards, List<FieldUnit> fieldUnits, FieldUnit unit, Player player) {
        if (handCards.Count + fieldUnits.Count != targets.Count) return false; //Way too few or way too many targets

        _handCards = handCards;
        _fieldUnits = fieldUnits;

        OnPlay (unit, player);

        return true;
    }
    protected abstract void OnPlay (FieldUnit unit, Player player);
}
