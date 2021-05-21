using UnityEngine;

[System.Serializable]
public class DeckCard
{
    [SerializeField] Card card;
    [SerializeField] int copies; 

    DeckCard (Card card) {
        this.card = card;
        this.copies = 1;
    }

    public void IncrementCopy () {
        copies++;
        copies = Mathf.Clamp (copies, 0, 3);
    }

    public void DecrementCopy () {
        copies--;
        copies = Mathf.Clamp (copies, 0, 3);
    }

    public int Copies {get {return copies;}}
    public Card Card {get {return card;}}
}
