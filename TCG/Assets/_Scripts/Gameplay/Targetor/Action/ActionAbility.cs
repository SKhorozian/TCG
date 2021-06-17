using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionAbility : Targetor
{
    protected FieldCard fieldCard;

    public FieldCard FieldCard {get {return fieldCard;} set {fieldCard = value;}}
}
