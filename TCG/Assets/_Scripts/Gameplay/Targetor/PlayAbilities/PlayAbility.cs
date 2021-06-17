using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayAbility : Targetor
{
    protected FieldCard playCard;

    public FieldCard FieldCard {get {return playCard;} set {playCard = value;}}
}
