using UnityEngine;
using System;
using TMPro;
using System.Collections;

public class BlackjackManager : MonoBehaviour
{
    [SerializeField] private CardSlot[] playerCardSlots;
    [SerializeField] private CardSlot[] dealerCardSlots;
    [SerializeField] private Sprite cardBackSprite;
    [SerializeField] private TextMeshProUGUI playerScoreText;
    [SerializeField] private TextMeshProUGUI dealerScoreText;
    [SerializeField] private TextMeshProUGUI gameResultText;

    private Action<float> onGameComplete;
    private bool gameActive = false;
    private bool resultSent = false;
    private int playerCardIndex;
    private int dealerCardIndex;
    private Deck deck;
    private Hand playerHand;
    private Hand dealerHand;
    private Card dealerHoleCard;

    void Awake()
    {
        deck = new Deck();
        playerHand = new Hand();
        dealerHand = new Hand();
    }

    public void StartNewGame(Action<float> onComplete)
    {
        playerCardIndex = 0;
        dealerCardIndex = 0;
        gameActive = true;
        resultSent = false;
        onGameComplete = onComplete;
        gameResultText.enabled = false;
        

        foreach (CardSlot pCardSlot in playerCardSlots) {
            pCardSlot.Initialize();
        }

        foreach (CardSlot dCardSlot in dealerCardSlots)
        {
            dCardSlot.Initialize();
        }

        playerHand.Clear();
        dealerHand.Clear();

        ShowCard(playerHand.AddCard(deck.DrawCard()), true);
        ShowCard(dealerHand.AddCard(deck.DrawCard()), false);
        ShowCard(playerHand.AddCard(deck.DrawCard()), true);
        dealerHoleCard = deck.DrawCard();
        dealerHand.AddCard(dealerHoleCard);
        ShowDealerCardBack();

        UpdateTextField(playerScoreText, "Score: ", playerHand.GetValue());
        UpdateTextField(dealerScoreText, "Score: ", dealerHand.GetValue() - dealerHand.GetCards()[1].GetValue());

        if (playerHand.IsBlackjack())
        {
            RevealDealerHoleCard();

            if (dealerHand.IsBlackjack())
            {
                // Draw
                EndGame(1f);
                return;
            }
            
            EndGame(3f);
            return;
        } else if (dealerHand.IsBlackjack())
        {
            EndGame(0f);
            return;
        }
    }

    public void Hit()
    {
        if (!gameActive) return;

        ShowCard(playerHand.AddCard(deck.DrawCard()), true);

        UpdateTextField(playerScoreText, "Score: ", playerHand.GetValue());

        if (playerHand.IsBust())
        {
            RevealDealerHoleCard();

            EndGame(0f);
            return;
        }
    }

    public void Stand()
    {
        if (!gameActive) return;

        StartCoroutine(DealerDrawCards());
    }

    private IEnumerator DealerDrawCards()
    {
        UpdateTextField(dealerScoreText, "Score: ", dealerHand.GetValue());
        RevealDealerHoleCard();
        yield return new WaitForSeconds(1.0f);

        while (dealerHand.GetValue() < 17)
        {
            yield return new WaitForSeconds(1.0f);

            ShowCard(dealerHand.AddCard(deck.DrawCard()), false);
            UpdateTextField(dealerScoreText, "Score: ", dealerHand.GetValue());
        }

        EvaluateGame();
    }

    private void EvaluateGame()
    {
        int playerValue = playerHand.GetValue();
        int dealerValue = dealerHand.GetValue();

        if (dealerHand.IsBust() || playerValue > dealerValue)
        {
            EndGame(2f);
            return;
        } else if (playerValue == dealerValue)
        {
            EndGame(1f);
            return;
        }

        EndGame(0f);
        return;
    }

    private void EndGame(float multiplier)
    {
        if (gameActive && !resultSent)
        {
            if (multiplier == 2f || multiplier == 3f)
            {
                ShowGameResultText("WIN", false);
            }
            else if (multiplier == 0f)
            {
                ShowGameResultText("LOSE", false);
            }
            else
            {
                ShowGameResultText(null, true);
            }
            
            resultSent = true;
            gameActive = false;

            //onGameComplete != null => Invoke (call) onGameComplete with multiplier
            onGameComplete?.Invoke(multiplier);
        }
    }

    private void ShowCard(Card card, bool isPlayer)
    {
        string spriteName = card.GetSpriteName();
        string[] parts = spriteName.Split('_');
        string suitName = parts[0];
        string spriteIndex = parts[1];

        Sprite[] sprites = Resources.LoadAll<Sprite>($"Cards/{suitName}");
        Sprite sprite = Array.Find(sprites, s => s.name == spriteName);

        if (sprite == null)
        {
            Debug.LogError($"Sprite not found: Cards/{spriteName}");
            return;
        }

        CardSlot[] slots = isPlayer ? playerCardSlots : dealerCardSlots;
        int currentIndex = isPlayer ? playerCardIndex : dealerCardIndex;

        if (currentIndex >= slots.Length)
        {
            Debug.LogError($"No more {(isPlayer ? "player" : "dealer")} card slots available!");
            return;
        }

        slots[currentIndex].ShowCard(sprite);

        if (isPlayer)
            playerCardIndex++;
        else
            dealerCardIndex++;
    }

    private void ShowDealerCardBack()
    {
        if (dealerCardIndex >= dealerCardSlots.Length)
        {
            Debug.LogError("No more dealer card slots available!");
            return;
        }

        if (cardBackSprite == null)
        {
            Debug.LogError("Card back sprite not assigned! Assign it in Inspector.");
            return;
        }

        dealerCardSlots[dealerCardIndex].ShowCard(cardBackSprite);
        dealerCardIndex++;
    }

    private void RevealDealerHoleCard()
    {
        int holeCardSlotIndex = 1;
        
        if (holeCardSlotIndex < dealerCardSlots.Length)
        {
            string spriteName = dealerHoleCard.GetSpriteName();
            string[] parts = spriteName.Split('_');
            string suitName = parts[0];

            Sprite[] sprites = Resources.LoadAll<Sprite>($"Cards/{suitName}");
            Sprite sprite = Array.Find(sprites, s => s.name == spriteName);

            if (sprite != null)
            {
                dealerCardSlots[holeCardSlotIndex].ShowCard(sprite);
            }
            else
            {
                Debug.LogError($"Could not find sprite for hole card: {spriteName}");
            }
        }
    }

    private void UpdateTextField(TextMeshProUGUI textField, string label, int value)
    {
        if (textField)
        {
            textField.text = $"{label}: {value}";
        }
    }

    private void ShowGameResultText(string label, bool draw)
    {
        gameResultText.enabled = true;
        if (gameResultText)
        {
            if (!draw)
            {
                gameResultText.text = $"YOU {label}!";
            } 
            else
            {
                gameResultText.text = "IT'S A DRAW!";
            }
        }
    }

    public void ResetGame() 
    {
        gameActive = false;
        resultSent = false;
    }
}
