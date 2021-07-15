using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "FishermanEntranceEffect", menuName = "Card Effects/Trigger/Blue/Units/FishermanEnteranceEffect")]
public class FishermanEntranceEffect : CardEffectTrigger
{
    public override CardEffect GetCardEffect()
    {
        return new DrawCardEffect (fieldCard.Player, 1);
    }
}
