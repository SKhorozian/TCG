using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetorStackUI : MonoBehaviour
{
    [SerializeField] TargetorStackIcon[] stackIcons;


    public void UpdateStack (Targetor[] targetors) {
        for (int i = 0; i < 10; i++) {
            if (i >= targetors.Length) stackIcons[i].gameObject.SetActive (false);
            else {
                stackIcons[i].gameObject.SetActive (true);
                stackIcons[i].UpdateTargetor (targetors[i]);
            }
        }
    }
}
