using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetorStackUI : MonoBehaviour
{
    [SerializeField] TargetorStackIcon[] stackIcons;
    int stackSize = 0;

    public void UpdateStack (Targetor[] targetors) {
        stackSize = targetors.Length;

        for (int i = 0; i < 10; i++) {
            if (i >= targetors.Length) stackIcons[i].gameObject.SetActive (false);
            else {
                stackIcons[i].gameObject.SetActive (true);
                stackIcons[i].UpdateTargetor (targetors[i]);
            }
        }
    }

    public int StackSize {get {return stackSize;}}
}
