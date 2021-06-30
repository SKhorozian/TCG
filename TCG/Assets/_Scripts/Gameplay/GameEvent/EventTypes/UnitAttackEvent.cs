public class UnitAttackEvent : GameEvent
{
    public UnitCardInstance attacker;
    public UnitCardInstance target;

    public UnitAttackEvent(Player player, int turnNumber, UnitCardInstance attacker, UnitCardInstance target) : base(player, turnNumber)
    {
        this.eventType = EventType.UnitAttack;
        this.attacker = attacker;
        this.target = target;
    }

    public UnitCardInstance Attacker    {get {return attacker;}}
    public UnitCardInstance Target      {get {return target;}}
}
