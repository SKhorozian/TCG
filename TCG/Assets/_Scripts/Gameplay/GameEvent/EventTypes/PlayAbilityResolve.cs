public class PlayAbilityResolve : GameEvent
{
    CardInstance cardInstance;
    PlayAbility playAbility;

    public PlayAbilityResolve(Player player, int turnNumber, CardInstance cardInstance, PlayAbility playAbility) : base(player, turnNumber)
    {
        this.eventType = EventType.PlayAbilityResolve;
        this.cardInstance = cardInstance;
        this.playAbility = playAbility;
    }

    public CardInstance CardInstance {get {return cardInstance;}}
    public PlayAbility PlayAbility   {get {return playAbility;}}
}
