public class PlayCardEvent : GameEvent
{
    CardInstance cardInstance;

    public PlayCardEvent(Player player, int turnNumber, CardInstance cardInstance) : base(player, turnNumber)
    {
        this.eventType = EventType.PlayCard;
        this.cardInstance = cardInstance;
    }

    public CardInstance CardInstance {get {return cardInstance;}}
}
