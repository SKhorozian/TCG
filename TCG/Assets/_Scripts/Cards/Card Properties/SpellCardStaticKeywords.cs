using UnityEngine;

[System.Flags]
public enum SpellCardStaticKeywords
{
    DoubleCast = (1 << 0),
    Lifesap = (1 << 1),
    Aftershock = (1 << 2),
}
