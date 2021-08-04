public class DrawSpecificCardEffect : CardEffect
{
    protected Card card;
    protected Player player;

    public DrawSpecificCardEffect (Card card, Player player) {
        this.card = card;
        this.player = player;
    }

    public override void DoEffect()
    {
        int cardPos = -1;
        
        for (int i = player.Deck.Count - 1; i > 0; i--) {
            if (player.Deck[i].Card.Equals (card)) {
                cardPos = i;
                break;
            }
        }

        if (cardPos < 0) return;
        
        player.DrawAt (cardPos);
    }
}
