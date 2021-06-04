using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action : Targetor
{
    [SerializeField, TextArea (minLines: 4, maxLines: 8)] string description;

    protected FieldCard playCard;

    public FieldCard FieldCard {get {return playCard;} set {playCard = value;}}
}
