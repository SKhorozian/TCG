public class SacrificeEvent : GameEvent
{
    CardInstance sacrifice;

    public SacrificeEvent(Player player, int turnNumber, CardInstance sacrifice) : base(player, turnNumber)
    {
        this.eventType = EventType.Sacrifice;
        this.sacrifice = sacrifice;
    }

    public CardInstance Sacrifice {get {return Sacrifice;}}
}
