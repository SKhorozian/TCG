using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Crystalize OnPlay", menuName = "Card OnPlay/Create New Crystalize OnPlay"), System.Serializable]
public class CrystalizeOnPlay : PlayAbility
{
    [SerializeField] int rampAmount;
    [SerializeField] bool refills;

    public override void DoEffect()
    {
        player.Crystalize (rampAmount);
        if (refills)
            player.RefillMana (rampAmount); 
    }

    public override bool TragetVaildity(int targetNumber, ITargetable target)
    {
        return true;
    }
}
