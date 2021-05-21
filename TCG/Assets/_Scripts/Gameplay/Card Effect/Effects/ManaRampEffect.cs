using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New ManaRamp Effect", menuName = "Card Effects/Create New ManaRamp Effect"), System.Serializable]
public class ManaRampEffect : CardEffect
{
    [SerializeField] int manaRampAmount;
    [SerializeField] bool refillsMana;
    [SerializeField] bool isPermanent;

    public override void DoEffect(FieldCard fieldCard)
    {
        //If it is a permanent ramp
        if (isPermanent) {
            fieldCard.Player.RampManaPermanent (manaRampAmount, refillsMana);
        } else { //Else call the temporary ramp one

        }
    }

}
