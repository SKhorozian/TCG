using UnityEngine;

[CreateAssetMenu (fileName = "New CreateCardInHand Action", menuName = "Action/Create new CreateCardInHand Action"), System.Serializable]
public class CreateCardInHandAction : ActionAbility
{
    [SerializeField] Card card;
    [SerializeField] bool isBrief;

    public override void DoEffect()
    {
        CardInstance createdCard = player.CreateCard (card);

        if (isBrief)
            createdCard.MakeBrief ();

        player.AddToHand (createdCard);
    }

    public override bool TragetVaildity(int targetNumber, ITargetable target)
    {
        return true;
    }
}
