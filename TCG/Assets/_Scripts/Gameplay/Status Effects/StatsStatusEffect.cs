public class StatsStatusEffect : StatusEffect
{   
    int bonusStrength;
    int bonusHealth;
    int bounsRange;
    int bonusSpeed;

    public StatsStatusEffect(StatusDuration duration, int bonusStrength, int bonusHealth, int bounsRange, int bonusSpeed) : base(duration)
    {
        this.bonusStrength = bonusStrength;
        this.bonusHealth = bonusHealth;
        this.bounsRange = bounsRange;
        this.bonusSpeed = bonusSpeed;
    }

    public override void ApplyStatus(FieldCard fieldCard)
    {
        if (!(fieldCard is FieldUnit)) return;
        
        FieldUnit unit = fieldCard as FieldUnit;

        unit.GiveStats (bonusStrength, bonusHealth, bounsRange, bonusSpeed);
    }

    public override void ApplyStatus(CardInstance card)
    {
        if (!(card is UnitCardInstance)) return;

        UnitCardInstance unitCard = card as UnitCardInstance;

        unitCard.GiveStats (bonusStrength, bonusHealth, bounsRange, bonusSpeed);
    }

    public override void RemoveStatus(FieldCard fieldCard)
    {
        if (!(fieldCard is FieldUnit)) return;
        
        FieldUnit unit = fieldCard as FieldUnit;

        unit.GiveStats (-bonusStrength, -bonusHealth, -bounsRange, -bonusSpeed);
    }

    public override void RemoveStatus(CardInstance card)
    {
        if (!(card is UnitCardInstance)) return;

        UnitCardInstance unitCard = card as UnitCardInstance;

        unitCard.GiveStats (-bonusStrength, -bonusHealth, -bounsRange, -bonusSpeed);
    }
}
