using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell : Targetor
{
    protected SpellCardInstance spellCardInstance;

    public SpellCardInstance SpellCard {get {return spellCardInstance;} set {spellCardInstance = value;}}

    public override string Location()
    {
        return "TargetorEffects/Spells/" + name;
    }
}
