using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaRampEffect : CardEffect
{
    int manaRampAmount;
    bool refillsMana;
    bool isPermanent;

    public ManaRampEffect(FieldCard fieldCard, int manaRampAmount, bool refillsMana, bool isPermanent) : base(fieldCard)
    {
        this.manaRampAmount = manaRampAmount;
        this.refillsMana = refillsMana;
        this.isPermanent = isPermanent;
    }

    public override void DoEffect()
    {
        //If it is a permanent ramp
        if (isPermanent) {
            fieldCard.Player.RampManaPermanent (manaRampAmount, refillsMana);
        } else { //Else call the temporary ramp one

        }
    }

}
