using UnityEngine;

[System.Flags]
public enum CardKeywords
{
    Decay = (1 << 0),
    Menace = (1 << 1),
    Stealth = (1 << 2)
}
