using UnityEngine;

[System.Flags]
public enum UnitCardStaticKeywords
{
    Decaying = (1 << 0),
    Menacing = (1 << 1),
    Stealth = (1 << 2),
    Scout = (1 << 3),
    Lifesap = (1 << 4),
    Savage = (1 << 5),
    Resonance = (1 << 6),
    A = (1 << 7),
    Undying = (1 << 8),
    Scavenger = (1 << 9),
}
