using UnityEngine;

[CreateAssetMenu (fileName = "New HealTargetDraw Spell", menuName = "Spell/Create New HealTargetDraw Spell"), System.Serializable]
public class HealTargetDrawSpell : HealTargetSpell
{
    [SerializeField] int drawAmount;

    public override void DoEffect()
    {
        base.DoEffect();
        player.Draw (drawAmount);
    }
}
