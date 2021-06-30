public class ActionResolveEvent : GameEvent
{
    FieldCard fieldCard;
    ActionAbility actionAbility;

    public ActionResolveEvent(Player player, int turnNumber, FieldCard fieldCard, ActionAbility actionAbility) : base(player, turnNumber)
    {  
        this.eventType = EventType.ActionResolve;
        this.fieldCard = fieldCard;
        this.actionAbility = actionAbility;
    }

    public FieldCard FieldCard         {get {return FieldCard;}}
    public ActionAbility ActionAbility {get {return actionAbility;}}
}
