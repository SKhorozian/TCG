using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New DrawCard Effect", menuName = "Card Effects/Create New DrawCard Effect"), System.Serializable]
public class DrawCardEffect : CardEffect
{
    [SerializeField] int drawAmount;

    public override void DoEffect()
    {
        fieldCard.Player.Draw (drawAmount);
    }
}
