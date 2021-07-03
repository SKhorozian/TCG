public class MovementAction : ActionAbility
{
    public MovementAction () {
        targetTypes = new TargetType[1];
        targetTypes[0] = TargetType.Hex;
    }

    public override void DoEffect()
    {
        HexagonCell hexCell = targets[0] as HexagonCell;

        FieldUnit mover = (fieldCard as FieldUnit);

        mover.Player.MoveUnit (mover, hexCell);

        mover.ConsumeActionPoint (1);

        mover.Player.MatchManage.eventTracker.AddEvent (new UnitMovementEvent (mover.Player, mover.Player.MatchManage.TurnNumber, mover.UnitsCard, hexCell));
    }

    public override bool TragetVaildity(int targetNumber, ITargetable target)
    {
        switch (targetNumber) {
            case 0: {
                if (!(target is HexagonCell)) return false; //If target isn't a hex cell
                if (!(fieldCard is FieldUnit)) return false;    //Field Card acting isn't a unit
                if ((target as HexagonCell).FieldCard != null) return false;    //If the cell is occupied

                FieldUnit fieldUnit = fieldCard as FieldUnit;

                if (HexagonMetrics.GetDistantce (fieldUnit.Cell.Position, (target as HexagonCell).Position) > fieldUnit.movementSpeed.Value) return false; //If it is outside the Unit's move range
                if (fieldUnit.currActionPoints.Value <= 0) return false;    //If the unit cannot act.
                return true;
            }
            default:
                return false;
        }
    }
}
