public class HealEvent : GameEvent
{
    Heal heal;
    CardInstance target;

    public HealEvent(Player player, int turnNumber, Heal heal, CardInstance target) : base(player, turnNumber)
    {
        this.eventType = EventType.Heal;
        this.heal = heal;
        this.target = target;
    }

    public Heal Heal {get {return heal;}}
    public CardInstance Target {get {return target;}}
}
