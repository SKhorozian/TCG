using UnityEngine;

[CreateAssetMenu (fileName = "JavelinTosserEntranceEffect", menuName = "Card Effects/Trigger/Yellow/Unit/JavelinTosserEntranceEffect")]
public class JavelinTosserEntranceEffect : CardEffectTrigger
{
    [SerializeField] Card card;

    public override CardEffect GetCardEffect()
    {
        return new JavelinTosserEffect (card, fieldCard.Player);
    }
}
