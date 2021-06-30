using MLAPI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();

            if (NetworkManager.Singleton.IsServer) {
                if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkedClient))
                {
                    var player = networkedClient.PlayerObject.GetComponent<Player>();
                    if (player)
                    {
                        if (!player.HasGameStart)
                            StartGame();
                    }
                }
            }

        }

        GUILayout.EndArea();
    }

    static void StartButtons()
    {
        if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
        if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
    }

    static void StatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
            NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
    }

    static void StartGame () {
        if (!NetworkManager.Singleton.IsServer) return;

        if (NetworkManager.Singleton.IsServer) {
            if (NetworkManager.Singleton.ConnectedClientsList.Count != 2) return;

            if (GUILayout.Button("Start Game"))
            {
                //Start the game
                //For each player start the game.
                foreach (MLAPI.Connection.NetworkClient c in NetworkManager.Singleton.ConnectedClientsList) {
                    var player = c.PlayerObject.GetComponent<Player>();
                    if (player)
                    {
                        player.StartGame();
                    }
                }
            }
        }
    }
    
}
