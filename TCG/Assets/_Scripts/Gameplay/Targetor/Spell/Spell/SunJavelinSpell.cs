using UnityEngine;

[CreateAssetMenu (fileName = "New SunJavelin Spell", menuName = "Spell/Create New SunJavelin Spell"), System.Serializable]
public class SunJavelinSpell : DamageTargetSpell
{
    [SerializeField] Card _card;

    public override void DoEffect()
    {
        int count = 0;

        foreach (CardInstance card in player.Junkyard) {
            if (card.Card.Equals (_card)) count++;
        }

        int newDamage = damageAmount + (damageAmount * count);

        IDamageable target = targets[0] as IDamageable;
        player.DamageTarget (target, new Damage (newDamage, DamageSource.Spell, player));
    }
}
