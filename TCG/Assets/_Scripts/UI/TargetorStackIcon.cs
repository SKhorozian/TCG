using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MLAPI;

public class TargetorStackIcon : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI descText;

    [SerializeField] Targetor targetor;

    public void UpdateTargetor (Targetor targetor) {
        if (!targetor) return;

        this.targetor = targetor;

        if (NetworkManager.Singleton.LocalClientId == targetor.Player.OwnerClientId) 
            icon.color = Color.green;
        else
            icon.color = Color.red;

        nameText.text = targetor.name;
        descText.text = targetor.Description;


    }

}
