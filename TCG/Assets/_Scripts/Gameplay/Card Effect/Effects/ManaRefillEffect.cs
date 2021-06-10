public class ManaRefillEffect : CardEffect
{
    int refillAmount;

    public ManaRefillEffect(FieldCard fieldCard, int refillAmount) : base(fieldCard)
    {
        this.refillAmount = refillAmount;
    }

    public override void DoEffect()
    {
        fieldCard.Player.RefillMana (refillAmount);
    }
}
