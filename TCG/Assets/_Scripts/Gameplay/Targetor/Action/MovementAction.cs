using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAction : Action
{
    public override void DoEffect()
    {
        HexagonCell hexCell = targets[0] as HexagonCell;

        fieldCard.Player.MoveUnit ((fieldCard as FieldUnit), hexCell);

        (fieldCard as FieldUnit).ConsumeActionPoint (1);
    }

    public override bool TragetVaildity(List<ITargetable> targets)
    {
        if (targets.Count != 1) return false;
        if (targets[0] == null) return false;
        if (!(targets[0] is HexagonCell)) return false; //If target isn't a hex cell
        if (!(fieldCard is FieldUnit)) return false;    //Field Card acting isn't a unit
        if ((targets[0] as HexagonCell).FieldCard != null) return false;    //If the cell is occupied

        FieldUnit fieldUnit = fieldCard as FieldUnit;

        if (HexagonMetrics.GetDistantce (fieldUnit.Cell.Position, (targets[0] as HexagonCell).Position) > fieldUnit.movementSpeed.Value) return false; //If it is outside the Unit's move range
        if (fieldUnit.currActionPoints.Value <= 0) return false;    //If the unit cannot act.

        return true;
    }
}
