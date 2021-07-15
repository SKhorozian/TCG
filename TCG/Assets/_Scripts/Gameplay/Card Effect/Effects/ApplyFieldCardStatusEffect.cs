public class ApplyFieldCardStatusEffect : CardEffect
{
    StatusEffect statusEffect;
    FieldCard target;

    public ApplyFieldCardStatusEffect (StatusEffect statusEffect, FieldCard target) {
        this.statusEffect = statusEffect;
        this.target = target;
    }

    public override void DoEffect()
    {
        if (target) {
            target.Card.AddStatusEffect (statusEffect);
            statusEffect.ApplyStatus(target);
        }
    }
}
