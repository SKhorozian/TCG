public abstract class StatusEffect
{
    StatusDuration duration;

    public StatusEffect (StatusDuration duration) {
        this.duration = duration;
    }

    //When the Status Effect is applyed.
    public abstract void ApplyStatus (FieldCard fieldCard);

    //When the Status Effect is removed.
    public abstract void RemoveStatus (FieldCard fieldCard);


    //When the Status Effect is applyed.
    public abstract void ApplyStatus (CardInstance card);

    //When the Status Effect is removed.
    public abstract void RemoveStatus (CardInstance card);

    

    public StatusDuration Duration {get {return duration;}}
}

public enum StatusDuration {
    TurnStart,
    TurnEnd,
    Permanent
}