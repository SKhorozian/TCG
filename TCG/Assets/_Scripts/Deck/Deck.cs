using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class for deck outside of games!!!!
[System.Serializable]
public class Deck
{
    [SerializeField] List<DeckCard> deck;

    public GameDeck InitializeDeck () {
        GameDeck gameDeck = new GameDeck ();

        foreach (DeckCard c in deck) {
            for (int i = 0; i < c.Copies; i++) {
                switch (c.Card.Type) {
                    case CardType.Unit:
                        gameDeck.Deck.Add (new UnitCardInstance (c.Card));
                        break;
                    case CardType.Spell:
                        gameDeck.Deck.Add (new SpellCardInstance (c.Card));
                        break;
                    case CardType.Structure:
                        break;
                    case CardType.Trap:
                        break;
                }


            }
        }

        gameDeck.ShuffleDeck ();

        return gameDeck;
    }

    public Deck (string deckCode) {
        deck = new List<DeckCard> ();
    }

    public Deck () {
        deck = new List<DeckCard> (); 
    }



}
