using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaRampEffect : CardEffect
{
    int manaRampAmount;
    bool refillsMana;
    bool isPermanent;
    Player player;

    public ManaRampEffect(int manaRampAmount, bool refillsMana, bool isPermanent, Player player) : base()
    {
        this.manaRampAmount = manaRampAmount;
        this.refillsMana = refillsMana;
        this.isPermanent = isPermanent;
        this.player = player;
    }

    public override void DoEffect()
    {
        //If it is a permanent ramp
        if (isPermanent) {
            player.RampManaPermanent (manaRampAmount, refillsMana);
        } else { //Else call the temporary ramp one

        }
    }

}
