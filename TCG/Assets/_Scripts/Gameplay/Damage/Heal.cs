using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal
{
    int amount;
    Player playerSource;

    public Heal (int amount, Player playerSource) {
        this.amount = amount;
        this.playerSource = playerSource;
    }

    public int HealAmount   {get {return amount;}}
    public Player PlayerSource  {get {return playerSource;}}
}
