using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "TheSunEffect", menuName = "Card Effects/Listener/Yellow/Structure/TheSunEffect")]
public class TheSunListenerEffect : CardEffectListener
{
    public override void RegisterListener(Player player)
    {
        player.preDamageEvent += TheSunEffect;
    }

    public override void RemoveListener(Player player)
    {
        player.preDamageEvent -= TheSunEffect;
    }

    void TheSunEffect (Damage damage, IDamageable target) {
        if (damage.Source != DamageSource.Spell) return;

        damage.DamageAmount += (int)(damage.BaseDamage * (0.1f + (0.1f * fieldCard.tallies.Value)) + 0.5f);
    }
}
