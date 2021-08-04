using UnityEngine;

public class JavelinTosserEffect : DrawSpecificCardEffect
{
    public JavelinTosserEffect (Card card, Player player) : base (card, player) {

    }

    public override void DoEffect()
    {
        bool condition = false;

        foreach (CardInstance card in player.Junkyard) {
            if (card.Card.Equals (this.card)) {
                condition = true;
                break;
            }
        }

        if (!condition) return;

        base.DoEffect();
    }
}
