public class UnitMovementEvent : GameEvent
{
    UnitCardInstance mover;
    HexagonCell to;

    public UnitMovementEvent(Player player, int turnNumber, UnitCardInstance mover, HexagonCell to) : base(player, turnNumber)
    {
        this.eventType = EventType.UnitMovement;
        this.mover = mover;
        this.to = to;
    }

    public UnitCardInstance Mover {get {return mover;}}
    public HexagonCell To         {get {return to;}}
}
