using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Spell", menuName = "Cards/Create New Spell"), System.Serializable]
public class SpellCard : Card
{
    [SerializeField] protected Spell spell;

    public Spell Spell {get {return spell;}}
}
