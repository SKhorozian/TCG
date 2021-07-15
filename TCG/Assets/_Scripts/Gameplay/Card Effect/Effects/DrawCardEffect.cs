public class DrawCardEffect : CardEffect
{
    int drawAmount;
    Player player;

    public DrawCardEffect(Player player, int drawAmount) : base()
    {
        this.player = player;
        this.drawAmount = drawAmount;
    }

    public override void DoEffect()
    {
        player.Draw (drawAmount);
    }
}
