using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New StrikeEachOther OnPlay", menuName = "Card OnPlay/Create New StrikeEachOther OnPlay"), System.Serializable]
public class StrikeEachOtherOnPlayEffect : PlayAbility
{
    public override void DoEffect()
    {
        if (targets[0] is FieldUnit) {
            if (playCard is FieldUnit) {
                (playCard as FieldUnit).Strike (targets[0] as FieldUnit);
                (targets[0] as FieldUnit).Strike ((playCard as FieldUnit));
            }
        }
    }

    public override bool TragetVaildity(int targetNumber, ITargetable target, Player player)
    {
        switch (targetNumber) {
            case 0: {
                if (!(target is FieldUnit)) return false;
                //It has to be an enemy unit
                if (playCard) //On Server
                    if ((target as FieldUnit).OwnerClientId.Equals (playCard.OwnerClientId)) return false;
                else //Client Side
                    if ((target as FieldUnit).IsOwner) return false;
                return true;
            }
            default:
                return false;
        }
    }
}
