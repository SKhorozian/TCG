public class DiscardCardEvent : GameEvent
{
    CardInstance discardedCard;

    public DiscardCardEvent(Player player, int turnNumber, CardInstance discardedCard) : base(player, turnNumber)
    {
        this.eventType = EventType.DiscardCard;
        this.discardedCard = discardedCard;
    }

    public CardInstance DiscardedCard {get {return discardedCard;}}
}
