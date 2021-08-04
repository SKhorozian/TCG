using MLAPI;
using MLAPI.Transports.UNET;
using UnityEngine;
using TMPro;

public class NetworkConnect : MonoBehaviour
{
    [SerializeField] GameObject inputObjs;
    [SerializeField] TMP_InputField addressInput;
    [SerializeField] TMP_InputField portInput;

    static string addressString;
    static string portString;

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            addressString = addressInput.text;
            portString = portInput.text;

            inputObjs.SetActive (true);
            StartButtons();
        }
        else
        {
            inputObjs.SetActive (false);
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
        int port = 0;

        int.TryParse (portString, out port);

        UNetTransport transport = NetworkManager.Singleton.GetComponent<UNetTransport>();

        transport.ConnectAddress = addressString; //takes string
        transport.ConnectPort = port;             //takes integer

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

        UNetTransport transport = NetworkManager.Singleton.GetComponent<UNetTransport>();
        GUILayout.Label("Address: " + transport.ConnectAddress);
        GUILayout.Label("Port: " + transport.ConnectPort);
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
