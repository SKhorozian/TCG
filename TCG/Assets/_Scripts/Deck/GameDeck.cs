using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameDeck
{
    [SerializeField] List<CardInstance> deck;

    public GameDeck () {
        deck = new List<CardInstance> ();
    }

    public void ShuffleDeck () {
        List<CardInstance> newDeck = new List<CardInstance> ();

        int deckSize = deck.Count;
        
        for (int i = 0; i < deckSize; i++) {
            int r = Random.Range (0, deck.Count);

            newDeck.Add(deck[r]);
            deck.RemoveAt(r);
        }

        deck = newDeck;
    }

    //Draw the topmost card from the deck
    public CardInstance Draw () {
        CardInstance card = deck[deck.Count-1];
        deck.RemoveAt (deck.Count-1);
        return card;
    }

    //Draw a specific card
    public CardInstance Draw (Card card) {
        return null;
    }

    //Draw a specific TYPE of card
    public CardInstance Draw (CardType type) {
        return null;
    }

    public List<CardInstance> Deck  {get{return deck;}}
    public int CurrentDeckSize      {get{return deck.Count;}}
}
