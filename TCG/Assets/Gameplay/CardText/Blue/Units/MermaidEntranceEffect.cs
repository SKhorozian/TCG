using UnityEngine;

[CreateAssetMenu (fileName = "MermaidEntranceEffect", menuName = "Card Effects/Trigger/Blue/Units/MermaidEnteranceEffect")]
public class MermaidEntranceEffect : CardEffectTrigger
{
    public override CardEffect GetCardEffect()
    {
        return new ManaRampEffect (1, false, true, fieldCard.Player);
    }
}
