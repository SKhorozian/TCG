using UnityEngine;

[CreateAssetMenu (fileName = "New BuffUnit Spell", menuName = "Spell/Create New BuffUnit Spell"), System.Serializable]
public class BuffUnitSpell : Spell
{
    [SerializeField] bool canTargetEnemy;

    [SerializeField] int strength;
    [SerializeField] int health;
    [SerializeField] int range;
    [SerializeField] int moveSpeed;

    public override void DoEffect()
    {
        StatsStatusEffect statusEffect = new StatsStatusEffect (StatusDuration.Permanent, strength, health, range, moveSpeed);
        ApplyFieldCardStatusEffect effect = new ApplyFieldCardStatusEffect (statusEffect, targets[0] as FieldCard);

        player.MatchManage.AddEffectToStack (effect);
    }

    public override bool TragetVaildity(int targetNumber, ITargetable target)
    {
        switch (targetNumber) {
            case 0: {
                if (!(target is FieldUnit)) return false;
                if (!(target as FieldUnit).Player.Equals (player) && !canTargetEnemy) return false; 

                return true;
            }
        }

        return false;
    }
}
