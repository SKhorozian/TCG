using UnityEngine;
using MLAPI;
using UnityEngine.UI;

public class TurnButton : MonoBehaviour
{

    [SerializeField] Image button;
    [SerializeField] Player player;

    void Start () {
        if (!player) {
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkedClient))
            {
                player = networkedClient.PlayerObject.GetComponent<Player>();
            }
        }
    }

    void Update () {

        if (!player) {
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkedClient))
            {
                player = networkedClient.PlayerObject.GetComponent<Player>();
            }
        }

        if (player) {
            if (player.MatchManage.LocalHasPriority) {
                button.color = Color.blue;
            } else {
                button.color = Color.gray;
            }
        }

    }

    public void TurnButtonPress () {
        if (player) {
            player.PassAction ();
        } else {
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkedClient))
            {
                player = networkedClient.PlayerObject.GetComponent<Player>();
                if (player)
                {
                    player.PassAction ();
                }
            }
        }

    }

}
