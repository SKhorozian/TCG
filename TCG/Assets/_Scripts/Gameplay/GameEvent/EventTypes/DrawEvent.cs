public class DrawEvent : GameEvent
{
    CardInstance drawnCard;

    public DrawEvent(Player player, int turnNumber, CardInstance drawnCard) : base(player, turnNumber)
    {
        this.eventType = EventType.Draw;
        this.drawnCard = drawnCard;
    }

    public CardInstance DrawnCard {get {return drawnCard;}}

}
