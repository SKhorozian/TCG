public class SpellResolveEvent : GameEvent
{  
    SpellCardInstance spellCardInstance;
    Spell spell;

    public SpellResolveEvent (Player player, int turnNumber, SpellCardInstance spellCardInstance, Spell spell) : base (player, turnNumber) {
        this.eventType = EventType.SpellResolve;
        this.spellCardInstance = spellCardInstance;
        this.spell = spell;
    }

    public SpellCardInstance SpellCardInstance {get {return spellCardInstance;}}
    public Spell Spell                         {get {return spell;}}
}
