using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Draw Spell", menuName = "Spell/Create New Draw Spell"), System.Serializable]
public class DrawCardSpell : Spell
{
    [SerializeField] int drawAmount;

    public override void DoEffect()
    {
        player.Draw (drawAmount);
    }

    public override bool TragetVaildity(List<ITargetable> targets)
    {
        return true;
    }
}
