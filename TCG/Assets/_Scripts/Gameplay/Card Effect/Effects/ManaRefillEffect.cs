public class ManaRefillEffect : CardEffect
{
    int refillAmount;
    Player player;

    public ManaRefillEffect(int refillAmount, Player player) : base()
    {
        this.refillAmount = refillAmount;
        this.player = player;
    }

    public override void DoEffect()
    {
        player.RefillMana (refillAmount);
    }
}
