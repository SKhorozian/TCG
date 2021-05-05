using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;

public class Player : NetworkBehaviour 
{
    public NetworkList<Card> playerHand = new NetworkList<Card> (new NetworkVariableSettings {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.OwnerOnly
    });

    public NetworkList<Card> playerDeck = new NetworkList<Card> (new NetworkVariableSettings{
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.ServerOnly
    });

    //Draws a number of cards
    public void Draw (int number) {
        if (NetworkManager.Singleton.IsHost) {

            //We Loop for the number of times we want to draw.
            for (int i = 0; i < number; i++) {
                if (playerDeck.Count <= 0 ) {
                    //No more cards in the deck
                    break;
                }

                playerHand.Add(playerDeck[playerDeck.Count-1]);
                playerDeck.RemoveAt(playerDeck.Count-1);
            }

        } else {
            //We call an RPC
        }
    }

    public void Draw (Card card) {
        //We draw a specific card
    }





}
