public class Act : GameEvent
{
    CardInstance actor;
    ActionAbility action;

    public Act(Player player, int turnNumber, CardInstance actor, ActionAbility action) : base(player, turnNumber)
    {
        this.eventType = EventType.Act;
        this.actor = actor;
        this.action = action;
    }

    public CardInstance Actor   {get {return actor;}}
    public ActionAbility Action {get {return action;}}
}
