using System.Collections.Generic;
public class Hand
{
    List<Card> hand = new List<Card>();

    public Card AddCard(Card card)
    {
        hand.Add(card);
        return card;
    }

    public void Clear()
    {
        hand.Clear();
    }

    public List<Card> GetCards()
    {
        return hand;
    }

    public int GetValue()
    {
        if (hand.Count == 0) return 0;

        int aceCount = 0;
        int handValue = 0;

        foreach (Card card in hand)
        {
            if (card.rank == Rank.Ace) aceCount++;

            handValue += card.GetValue();
        }

        while (handValue > 21 && aceCount > 0)
        {
            handValue -= 10;
            aceCount--;
        }

        return handValue;
    }

    public bool IsBust()
    {
        return GetValue() > 21;
    }

    public bool IsBlackjack()
    {
        // 2-card 21?
        return GetValue() == 21 && hand.Count == 2;
    }
    public int CardCount()
    {
        return hand.Count;
    }

    public string GetHandString()
    {
        if (hand.Count == 0) return "Empty hand";

        string result = "";
        foreach (Card card in hand)
        {
            result += card.ToString() + ", ";
        }
        return result.TrimEnd(',', ' ');
    }
}
