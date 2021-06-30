public class BanishCardEvent : GameEvent
{
    CardInstance banishedCard;

    public BanishCardEvent(Player player, int turnNumber, CardInstance banishedCard) : base(player, turnNumber)
    {
        this.banishedCard = banishedCard;
    }

    public CardInstance BanishedCard {get {return banishedCard;}}
}
