using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Conquer Action", menuName = "Action/Create New Conquer Action"), System.Serializable]
public class ConquerHexAction : ActionAbility
{
    [SerializeField] CardColor color;

    public ConquerHexAction () {
        targetTypes = new TargetType[1];
        targetTypes[0] = TargetType.Hex;
        speed = TargetorPriority.Ritual;
    }

    public override void DoEffect()
    {
        (targets[0] as HexagonCell).ConquerHex (color, fieldCard.Player);
        fieldCard.Player.FieldHero.ConsumeActionPoint();
    }

    public override bool TragetVaildity(int targetNumber, ITargetable target)
    {
        switch (targetNumber) {
            case 0: {
                if (target !is HexagonCell)  return false; //It has to be a HexCell
                
                HexagonCell cell = target as HexagonCell;
                
                if (cell.ConqueredPlayer) return false;
                
                return true;
            }
        }

        return false;
    }
}
