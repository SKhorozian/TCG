using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatsDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI healthNumber;
    [SerializeField] TextMeshProUGUI manaNumber;

    [SerializeField] Image[] manaIcons;

    public void UpdateDisplay (int health, int mana, int maxMana) {
        healthNumber.text = health.ToString();
        manaNumber.text = mana.ToString() + "/" +maxMana.ToString();

        for (int i = 0; i < 10; i++) {
            if (i > maxMana - 1) {
                manaIcons[i].color = Color.grey;
            } else if (i > mana - 1) {
                manaIcons[i].color = Color.black;
            } else {
                manaIcons[i].color = Color.blue;
            }
        }
    }

}
