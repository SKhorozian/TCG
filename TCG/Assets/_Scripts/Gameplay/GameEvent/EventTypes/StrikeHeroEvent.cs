public class StrikeHeroEvent : GameEvent
{
    UnitCardInstance striker;
    FieldHero target;

    public StrikeHeroEvent(Player player, int turnNumber, UnitCardInstance striker, FieldHero target) : base(player, turnNumber)
    {
        this.eventType = EventType.StrikeHero;
        this.striker = striker;
        this.target = target;
    }

    public UnitCardInstance Striker  {get {return striker;}}
    public FieldHero Target          {get {return target;}}
}
