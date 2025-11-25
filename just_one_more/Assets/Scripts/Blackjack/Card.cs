public struct Card
{
    public Suit suit;
    public Rank rank;

    public Card(Suit suit, Rank rank)
    {
        this.suit = suit;
        this.rank = rank;
    }

    public int GetValue()
    {
        if (rank >= Rank.Two && rank <= Rank.Ten)
        {
            return (int) rank;
        } else if (rank >= Rank.Jack)
        {
            return 10;
        }
        // If here, the card is Ace
        return 11;
    }

    public override string ToString()
    {
        return $"{rank} of {suit}";
    }

    public string GetSpriteName()
    {
        return $"{rank}_{suit}";
    }
}
