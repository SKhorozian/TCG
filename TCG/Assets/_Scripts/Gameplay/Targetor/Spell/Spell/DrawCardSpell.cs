using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Draw Spell", menuName = "Spell/Create New Draw Spell"), System.Serializable]
public class DrawCardSpell : Spell
{
    [SerializeField] int drawAmount;

    public override void DoEffect()
    {
        player.MatchManage.AddEffectToStack (new DrawCardEffect (player, 2));
    }

    public override bool TragetVaildity(int targetNumber, ITargetable target)
    {
        return true;
    }
}
