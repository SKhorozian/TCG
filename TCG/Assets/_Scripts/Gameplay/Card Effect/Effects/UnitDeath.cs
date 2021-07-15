public class UnitDeath : CardEffect
{
    FieldUnit dyingUnit;

    public UnitDeath(FieldUnit dyingUnit) : base()
    {
        this.dyingUnit = dyingUnit;
    }

    public override void DoEffect()
    {
        dyingUnit.Player.UnitDie (dyingUnit);
    }
}
