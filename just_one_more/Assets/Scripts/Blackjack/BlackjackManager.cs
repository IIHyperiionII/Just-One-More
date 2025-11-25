using UnityEngine;
using System;
using UnityEditor.PackageManager;

public class BlackjackManager : MonoBehaviour
{
    [SerializeField] private CardSlot[] playerCardSlots;
    [SerializeField] private CardSlot[] dealerCardSlots;
    private Action<float> onGameComplete;
    private bool gameActive = false;
    private bool resultSent = false;
    private int playerCardIndex;
    private int dealerCardIndex;
    private Deck deck;
    private Hand playerHand;
    private Hand dealerHand;

    void Awake()
    {
        deck = new Deck();
        playerHand = new Hand();
        dealerHand = new Hand();
    }

    void Update()
    {
        if (!gameActive) return;

        if (Input.GetKeyDown(KeyCode.H))
        {
            Hit();
        } else if (Input.GetKeyDown(KeyCode.S))
        {
            Stand();
        }
    }

    public void StartNewGame(Action<float> onComplete)
    {
        playerCardIndex = 0;
        dealerCardIndex = 0;
        gameActive = true;
        resultSent = false;
        onGameComplete = onComplete;

        foreach (CardSlot pCardSlot in playerCardSlots) {
            pCardSlot.Initialize();
        }

        foreach (CardSlot dCardSlot in dealerCardSlots)
        {
            dCardSlot.Initialize();
        }

        playerHand.Clear();
        dealerHand.Clear();

        ShowPlayerCard(playerHand.AddCard(deck.DrawCard()));
        ShowDealerCard(dealerHand.AddCard(deck.DrawCard()));
        ShowPlayerCard(playerHand.AddCard(deck.DrawCard()));
        ShowDealerCard(dealerHand.AddCard(deck.DrawCard()));

        Debug.Log("===== NEW BLACKJACK GAME =====");
        Debug.Log($"Player: {playerHand.GetHandString()} = {playerHand.GetValue()}");

        Card dealerFirstCard = dealerHand.GetCards()[0];
        Debug.Log($"Dealer shows: {dealerFirstCard} (2nd card hidden)");

        if (playerHand.IsBlackjack())
        {
            if (dealerHand.IsBlackjack())
            {
                // Draw
                EndGame(1f);
            }
            
            EndGame(3f);
        } else if (dealerHand.IsBlackjack())
        {
            EndGame(0f);
        }

        // Wait for input
        Debug.Log("Press H to hit and S to stand. Have fun!");
    }

    public void Hit()
    {
        if (!gameActive) return;

        ShowPlayerCard(playerHand.AddCard(deck.DrawCard()));
        Debug.Log("Player hits!");
        Debug.Log($"Players new hand: {playerHand.GetHandString()} = {playerHand.GetValue()}");

        if (playerHand.IsBust())
        {
            Debug.Log($"Player busted: {playerHand.GetValue()}");
            EndGame(0f);
        }
    }

    public void Stand()
    {
        if (!gameActive) return;

        Debug.Log("Player stands!");
        Debug.Log($"Dealers reveals: {dealerHand.GetHandString()} = {dealerHand.GetValue()}");

        while (dealerHand.GetValue() < 17)
        {
            ShowDealerCard(dealerHand.AddCard(deck.DrawCard()));

            Debug.Log("Dealer hits!");
            Debug.Log($"Dealers new hand: {dealerHand.GetHandString()} = {dealerHand.GetValue()}");
        }

        Debug.Log("Dealer stands!");
        EvaluateGame();
    }

    private void EvaluateGame()
    {
        int playerValue = playerHand.GetValue();
        int dealerValue = dealerHand.GetValue();

        Debug.Log($"Player = {playerValue} x {dealerValue} = Dealer");

        if (dealerHand.IsBust() || playerValue > dealerValue)
        {
            EndGame(2f);
        } else if (playerValue == dealerValue)
        {
            EndGame(1f);
        }

        EndGame(0f);
    }

    private void EndGame(float multiplier)
    {
        if (gameActive && !resultSent)
        {
            Debug.Log($"Player gets {multiplier} multiplier.");
            Debug.Log("===== BLACKJACK GAME ENDS =====");
            
            resultSent = true;
            gameActive = false;

            //onGameComplete != null => Invoke (call) onGameComplete with multiplier
            onGameComplete?.Invoke(multiplier);
        }
    }

    private void ShowPlayerCard(Card card)
    {
        string spriteName = card.GetSpriteName();
        Sprite sprite = Resources.Load<Sprite>($"Cards/{spriteName}");

        if (sprite == null)
        {
            Debug.LogError($"Sprite not found: Cards/{spriteName}");
            return;
        }

        if (playerCardIndex >= playerCardSlots.Length)
        {
            Debug.LogError("No more player card slots available!");
            return;
        }

        playerCardSlots[playerCardIndex].ShowCard(sprite);
        playerCardIndex++;
    }

    private void ShowDealerCard(Card card)
    {
        string spriteName = card.GetSpriteName();
        Sprite sprite = Resources.Load<Sprite>($"Cards/{spriteName}");

        if (sprite == null)
        {
            Debug.LogError($"Sprite not found: Cards/{spriteName}");
            return;
        }

        if (dealerCardIndex >= dealerCardSlots.Length)
        {
            Debug.LogError("No more dealer card slots available!");
            return;
        }

        dealerCardSlots[dealerCardIndex].ShowCard(sprite);
        dealerCardIndex++;
    }

    public void ResetGame() 
    {
        gameActive = false;
        resultSent = false;
    }
}
