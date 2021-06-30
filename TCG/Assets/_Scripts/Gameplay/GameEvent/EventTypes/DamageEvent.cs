public class DamageEvent : GameEvent
{
    Damage damage;

    public DamageEvent(Player player, int turnNumber, Damage damage) : base(player, turnNumber)
    {
        this.eventType = EventType.Damage;
        this.damage = damage;
    }

    public Damage Damage    {get {return damage;}}
}
