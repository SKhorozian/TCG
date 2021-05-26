using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetable
{

}

public static class Target {

    public static bool isCard (ITargetable target) {return target is CardInstance;}

    public static CardInstance CardInstance (ITargetable target) {
        if (!isCard (target)) return null;
        return target as CardInstance;
    }

}