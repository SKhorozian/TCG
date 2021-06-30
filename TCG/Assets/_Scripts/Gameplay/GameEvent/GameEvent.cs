public abstract class GameEvent
{
    protected EventType eventType;
    Player player;
    int turnNumber;

    public GameEvent (Player player, int turnNumber) {
        this.player = player;
        this.turnNumber = turnNumber;
    }

    public EventType EventType {get {return eventType;}}
    public Player Player       {get {return player;}}
    public int TurnNumber      {get {return turnNumber;}}
}

public enum EventType {
    ActionResolve,
    SpellResolve,
    PlayAbilityResolve,
    Strike,
    PlayCard,
    SummonUnit,
    SummonStructure,
    DiscardCard,
    SpendMana,
    UnitDeath,
    StructureDemolish,
    UnitAttack,
    UnitMovement,
    Act,
    Draw,
    CardCreated,
    Damage,
    Sacrifice,
    AttackHero,
    StrikeHero,
    Banish,
    Heal
}
