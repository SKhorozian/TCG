public class SpendManaEvent : GameEvent
{
    int amount;

    public SpendManaEvent(Player player, int turnNumber, int amount) : base(player, turnNumber)
    {
        this.eventType = EventType.SpendMana;
        this.amount = amount;
    }

    public int Amount {get {return amount;}}
}
