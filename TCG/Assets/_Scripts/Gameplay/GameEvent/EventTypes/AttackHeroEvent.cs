public class AttackHeroEvent : GameEvent
{
    UnitCardInstance attacker;
    FieldHero target;

    public AttackHeroEvent(Player player, int turnNumber, UnitCardInstance attacker, FieldHero target) : base(player, turnNumber)
    {
        this.eventType = EventType.AttackHero;
        this.attacker = attacker;
        this.target = target;
    }

    public UnitCardInstance Attacker {get {return attacker;}}
    public FieldHero Target          {get {return target;}}
}