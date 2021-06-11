using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell : Targetor
{
    protected SpellCardInstance spellCardInstance;
    protected Player player;

    public SpellCardInstance SpellCard {get {return spellCardInstance;} set {spellCardInstance = value;}}
    public Player Player {get {return player;} set {player = value;}}
}
