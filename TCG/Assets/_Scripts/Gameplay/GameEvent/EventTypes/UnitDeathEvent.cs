public class UnitDeathEvent : GameEvent
{
    UnitCardInstance deadUnit;

    public UnitDeathEvent(Player player, int turnNumber, UnitCardInstance deadUnit) : base(player, turnNumber)
    {
        this.eventType = EventType.UnitDeath;
        this.deadUnit = deadUnit;
    }

    public UnitCardInstance DeadUnit {get {return deadUnit;}}
}
