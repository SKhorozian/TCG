public class StructureDemolition : CardEffect
{
    public StructureDemolition(FieldCard fieldCard) : base(fieldCard)
    {

    }

    public override void DoEffect()
    {
        if (!(fieldCard is FieldStructure)) return;     
        FieldStructure structure = fieldCard as FieldStructure;

        structure.Player.StructureDemolish (structure);
    }
}
