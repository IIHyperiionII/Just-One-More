using UnityEngine;
using System.Collections.Generic;
using System;

// Card deck with shuffle functionality

public class Deck
{
    private List<Card> cards = new List<Card>();

    public Deck()
    {
        ResetDeck();
    }

    public void ResetDeck()
    {
        cards.Clear();

        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {
            foreach (Rank rank in Enum.GetValues(typeof(Rank)))
            {
                cards.Add(new Card(suit, rank));
            }
        }

        Shuffle();
    }

    public void Shuffle()
    {
        // Fisher-Yates shuffle
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);

            Card temp = cards[i];
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
    }

    // Returns card and resets deck if all cards were taken
    public Card DrawCard()
    {
        if (cards.Count == 0)
        {
            ResetDeck();
        }
        Card drawnCard = cards[0];

        cards.RemoveAt(0);

        return drawnCard;
    }

    public int CardsRemaining()
    {
        return cards.Count;
    }
}
