public class StructureDemolishEvent : GameEvent
{
    StructureCardInstance demolishedStructure;

    public StructureDemolishEvent(Player player, int turnNumber, StructureCardInstance demolishedStructure) : base(player, turnNumber)
    {
        this.eventType = EventType.StructureDemolish;
        this.demolishedStructure = demolishedStructure;
    }

    public StructureCardInstance DemolishedStructure {get {return demolishedStructure;}}
}
