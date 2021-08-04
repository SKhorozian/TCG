using UnityEngine;
using TMPro;
using MLAPI;
using UnityEngine.UI;

public class TurnButton : MonoBehaviour
{

    [SerializeField] Image button;
    [SerializeField] TextMeshProUGUI buttonText;
    [SerializeField] Player player;

    [SerializeField] bool canPress;
    [SerializeField] PlayerController playerController;

    void Update () {

        if (!player) {
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkedClient))
            {
                player = networkedClient.PlayerObject.GetComponent<Player>();
            }
        }

        canPress = CheckIfCanPress ();

        //The button will be grey if the player can't press it, and blue if they can.
        button.color = canPress ? Color.blue : Color.gray;
    }

    bool CheckIfCanPress () {
        if (player) {
            if (!player.MatchManage.LocalPlayerPriority) {
                buttonText.text = "Enemy's Priority";
                return false;
            }
        }

        if (playerController.IsFocused) {
            buttonText.text = "...";
            return false;
        }

        if (playerController.StackUI.StackSize > 0) {
            if (player.MatchManage.HasActed)
                buttonText.text = "Pass Priority";
            else
                buttonText.text = "Resolve Stack";
        } else {
            buttonText.text = "End Turn";
        }

        return true;
    }

    public void TurnButtonPress () {
        if (!CheckIfCanPress ()) return;

        if (player) {
            player.ButtonPress ();
        } else {
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkedClient))
            {
                player = networkedClient.PlayerObject.GetComponent<Player>();
                if (player)
                {
                    player.ButtonPress ();
                }
            }
        }

    }

}
