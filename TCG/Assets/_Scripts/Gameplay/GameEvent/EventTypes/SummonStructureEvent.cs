public class SummonStructureEvent : GameEvent
{
    StructureCardInstance structureSummoned;

    public SummonStructureEvent(Player player, int turnNumber, StructureCardInstance structureSummoned) : base(player, turnNumber)
    {
        this.eventType = EventType.SummonStructure;
        this.structureSummoned = structureSummoned;
    }

    public StructureCardInstance StructureSummoned {get {return structureSummoned;}}
}
