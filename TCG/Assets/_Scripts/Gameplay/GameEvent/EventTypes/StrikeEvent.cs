public class StrikeEvent : GameEvent
{
    UnitCardInstance striker;
    UnitCardInstance target;
    Damage damage;

    public StrikeEvent(Player player, int turnNumber, UnitCardInstance striker, UnitCardInstance target, Damage damage) : base(player, turnNumber)
    {
        this.eventType = EventType.Strike;
        this.striker = striker;
        this.target = target;
        this.damage = damage;
    }

    public UnitCardInstance Striker     {get {return striker;}}
    public UnitCardInstance Target      {get {return target;}}
    public Damage Damage                {get {return damage;}}
}
