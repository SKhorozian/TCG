using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New ColossalGolem Effect", menuName = "Card Effects/Create New ColossalGolem Effect"), System.Serializable]
public class ColossalGolemEffect : CardEffect
{
    public override void DoEffect()
    {
        foreach (FieldUnit unit in fieldCard.Player.MatchManage.AllUnits) {
            if (!unit.Equals (fieldCard)) {
                unit.TakeDamage (new Damage (100, DamageSource.Effect, fieldCard.Player));
            }
        }
    }
}
