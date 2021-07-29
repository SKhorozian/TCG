public class AttackAction : ActionAbility
{
    public AttackAction () {
        targetTypes = new TargetType[1];
        targetTypes[0] = TargetType.FieldCard;
        speed = TargetorPriority.Ritual;
    }

    public override void DoEffect()
    {
        FieldUnit attacker = fieldCard as FieldUnit;

        attacker.Strike (targets[0] as IDamageable);

        if (targets[0] is FieldUnit) {
            FieldUnit target = targets[0] as FieldUnit;

            if (target.currActionPoints.Value > 0 && (HexagonMetrics.GetDistantce (attacker.Cell.Position, target.Cell.Position) <= target.attackRange.Value)) target.Strike (attacker);
        }
    }

    public override bool TragetVaildity(int targetNumber, ITargetable target)
    {
        if (!(fieldCard is FieldUnit)) return false;

        switch (targetNumber) {
            case 0: {
                if (!(target is IDamageable)) return false; //Target must be damageable

                if (target.Equals (fieldCard)) return false; //Cannot attack self
                
                if ((target as FieldCard).OwnerClientId.Equals (player.OwnerClientId)) return false; //Cannot attack friendly units

                if (HexagonMetrics.GetDistantce (fieldCard.Cell.Position, (target as FieldCard).Cell.Position) > (fieldCard as FieldUnit).attackRange.Value) return false; // Check for distance

                if (target is FieldUnit) //Check if this unit can attack it
                    if (!player.MatchManage.CheckCanAttack (fieldCard as FieldUnit, target as FieldUnit)) return false;

                return true;
            }
            default:
                return false;
        }
    }
}
