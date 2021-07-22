using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalizeEffect : CardEffect
{
    int manaRampAmount;
    bool refillsMana;
    Player player;

    public CrystalizeEffect(int manaRampAmount, bool refillsMana, Player player) : base()
    {
        this.manaRampAmount = manaRampAmount;
        this.refillsMana = refillsMana;
        this.player = player;
    }

    public override void DoEffect()
    {
        player.Crystalize (manaRampAmount);
        if (refillsMana)
            player.RefillMana (manaRampAmount);
    }

}
