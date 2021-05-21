using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHandDisplay : MonoBehaviour
{
    [SerializeField] List<CardDisplay> cardDisplays;
    [SerializeField] GridLayoutGroup gridLayoutGroup;

    public void UpdateCardDisplays (CardInstance[] handCards) {
        for (int i = 0; i < cardDisplays.Count; i++) {
            cardDisplays[i].gameObject.SetActive (false);
        }

        for (int i = 0; i < handCards.Length; i++) {
            cardDisplays[i].gameObject.SetActive (true);

            cardDisplays[i].SetCard(handCards[i]);
        }
    } 

}
