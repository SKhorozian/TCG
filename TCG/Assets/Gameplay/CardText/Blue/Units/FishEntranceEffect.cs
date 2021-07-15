using UnityEngine;

[CreateAssetMenu (fileName = "FishEntranceEffect", menuName = "Card Effects/Trigger/Blue/Units/FishEnteranceEffect")]
public class FishEntranceEffect : CardEffectTrigger
{
    public override CardEffect GetCardEffect()
    {
        return new ManaRefillEffect (1, fieldCard.Player);
    }
}
