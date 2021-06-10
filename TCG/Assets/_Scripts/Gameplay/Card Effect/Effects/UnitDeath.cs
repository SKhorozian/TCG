public class UnitDeath : CardEffect
{
    public UnitDeath(FieldCard fieldCard) : base(fieldCard)
    {
    }

    public override void DoEffect()
    {
        if (!(fieldCard is FieldUnit)) return;     
        FieldUnit unit = fieldCard as FieldUnit;

        unit.Player.UnitDie (unit);
    }
}
