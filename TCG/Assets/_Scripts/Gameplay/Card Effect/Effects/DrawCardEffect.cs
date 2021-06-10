public class DrawCardEffect : CardEffect
{
    int drawAmount;

    public DrawCardEffect(FieldCard fieldCard, int drawAmount) : base(fieldCard)
    {
        this.drawAmount = drawAmount;
    }

    public override void DoEffect()
    {
        fieldCard.Player.Draw (drawAmount);
    }
}
