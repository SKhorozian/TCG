public class SummonUnitEvent : GameEvent
{
    UnitCardInstance unitSummoned;

    public SummonUnitEvent(Player player, int turnNumber, UnitCardInstance unitSummoned) : base(player, turnNumber)
    {
        this.eventType = EventType.SummonUnit;
        this.unitSummoned = unitSummoned;
    }

    public UnitCardInstance UnitSummoned {get {return unitSummoned;}}
}
