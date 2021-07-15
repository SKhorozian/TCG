public class StructureDemolition : CardEffect
{
    FieldStructure demolishingStructure;

    public StructureDemolition(FieldStructure demolishingStructure) : base()
    {
        this.demolishingStructure = demolishingStructure;
    }

    public override void DoEffect()
    {
        demolishingStructure.Player.StructureDemolish (demolishingStructure);
    }
}
