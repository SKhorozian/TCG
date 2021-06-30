public class CardCreatedEvent : GameEvent
{
    CardInstance createdCard;

    public CardCreatedEvent(Player player, int turnNumber, CardInstance createdCard) : base(player, turnNumber)
    {
        this.eventType = EventType.CardCreated;
        this.createdCard = createdCard;
    }   

    public CardInstance CreatedCard {get {return createdCard;}}
}
