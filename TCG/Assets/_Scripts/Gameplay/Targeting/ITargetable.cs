using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetable
{

}

public enum TargetType {
    Hex,
    FieldCard,
    Trap,
    Stack,
    Hand
}

public static class Targeting {
    public static List<ITargetable> ConvertTargets (TargetType[] targetTypes, Vector2[] targets, Player player) {
        List<ITargetable> newTargets = new List<ITargetable> ();
        if (targetTypes.Length != targets.Length) return newTargets;

        for (int i = 0; i < targets.Length; i++) {
            switch (targetTypes[i]) {
                case TargetType.Hex: {
                    HexagonCell cell;
                        if (player.MatchManage.FieldGrid.Cells.TryGetValue(targets[i], out cell)) {
                            newTargets.Add (cell);
                        } else {Debug.Log ("Targeting Conversion: Couldn't find Hex"); return newTargets;}
                    break;
                }
                case TargetType.FieldCard: {
                    HexagonCell cell;
                    if (player.MatchManage.FieldGrid.Cells.TryGetValue(targets[i], out cell)) {
                        if (cell.FieldCard)
                            newTargets.Add (cell.FieldCard);
                        else {Debug.Log ("Targeting Conversion: Couldn't find FieldCard"); return newTargets;}
                    } else {Debug.Log ("Targeting Conversion: Couldn't find Hex"); return newTargets;}
                    break;
                }
                case TargetType.Hand: {
                    if (targets[i].y > 10 && targets[i].y > 10) {Debug.Log ("Card hand number out of bounds"); return newTargets;}
                    newTargets.Add (player.playerHand[ (int)targets[i].y ] );
                    break;
                }
                case TargetType.Trap: {

                    break;
                }
                case TargetType.Stack: {

                    break;
                }
            } 
        }

        return newTargets;
    }
}