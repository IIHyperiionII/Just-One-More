using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public enum StatType
{
    Money,
    HP,
    Dmg,
    Speed
}

public class CasinoManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI betAmountText;
    [SerializeField] private TextMeshProUGUI selectedStatText;
    [SerializeField] private Slider betSlider;

    [Header("Stat Selection Buttons")]
    [SerializeField] private Button moneyButton;
    [SerializeField] private Button hpButton;
    [SerializeField] private Button damageButton;
    [SerializeField] private Button speedButton;

    [Header("Panels")]
    [SerializeField] private GameObject casinoPanel;
    [SerializeField] private GameObject gamblingPanel;

    [Header("Data")]
    [SerializeField] private PlayerData playerData;
    [SerializeField] private PlayerStatsPanel playerStatsPanel;

    [Header("Game References")]
    [SerializeField] private GamblingManager gamblingManager;

    [Header("Settings")]
    [SerializeField] private int minBet = 1;
    [SerializeField] private int betIncrement = 10;
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color normalColor = Color.white;

    // private int currentStat;
    private int currentBet;
    private StatType currentStatType = StatType.Money;
    private bool gameInProgress = false;

    void Start()
    {   
        // RESET FOR TESTING
        if (playerData != null)
        {
            playerData.money = 1000;
            playerData.hp = 100;
            playerData.damage = 40;
            playerData.moveSpeed = 20;
            Debug.Log("PlayerData reset for testing!");
        }

        currentBet = minBet;
        SetupSlider();

        if (gamblingPanel != null)
        {
            gamblingPanel.SetActive(false);
        }

        UpdateStatButtonColors();
        UpdateUI();
    }

    // ========== SETUP ==========
    
    private void SetupSlider()
    {
        if (betSlider != null)
        {
            betSlider.wholeNumbers = true;
            betSlider.minValue = minBet;
            betSlider.maxValue = GetMaxBetForCurrentStat();
            betSlider.SetValueWithoutNotify(currentBet);
            betSlider.onValueChanged.AddListener(OnSliderChanged);
        }
    }

    // ========== STAT SELECTION (PUBLIC - pro OnClick) ==========
    public void SelectMoney()
    {
        SelectStat(StatType.Money);
    }

    public void SelectHP()
    {
        SelectStat(StatType.HP);
    }

    public void SelectDamage()
    {
        SelectStat(StatType.Dmg);
    }

    public void SelectSpeed()
    {
        SelectStat(StatType.Speed);
    }
    
    private void SelectStat(StatType statType)
    {
        if (!gameInProgress)
        {
            currentStatType = statType;
            currentBet = minBet;
            
            if (betSlider != null)
            {
                betSlider.maxValue = GetMaxBetForCurrentStat();
                betSlider.SetValueWithoutNotify(currentBet);
            }
            
            UpdateStatButtonColors();
            UpdateUI();
        }
    }

    private void UpdateStatButtonColors()
    {
        SetButtonColor(moneyButton, currentStatType == StatType.Money);
        SetButtonColor(hpButton, currentStatType == StatType.HP);
        SetButtonColor(damageButton, currentStatType == StatType.Dmg);
        SetButtonColor(speedButton, currentStatType == StatType.Speed);
    }

    // Weird implementation idk
    private void SetButtonColor(Button button, bool isSelected)
    {
        if (button != null)
        {
            ColorBlock colors = button.colors;
            colors.normalColor = isSelected ? selectedColor : normalColor;
            colors.selectedColor = isSelected ? selectedColor : normalColor;
            button.colors = colors;
        }
    }

    private int GetMaxBetForCurrentStat()
    {
        if (playerData == null) return minBet;

        switch (currentStatType)
        {
            case StatType.Money:
                return playerData.money;
            case StatType.HP:
                return playerData.hp;
            case StatType.Dmg:
                return playerData.damage;
            case StatType.Speed:
                return Mathf.FloorToInt(playerData.moveSpeed);
            default:
                return minBet;
        }
    }
    
    // ========== BETTING ==========

    private void OnSliderChanged(float value)
    {
        if (!gameInProgress)
        {
            currentBet = Mathf.RoundToInt(value);
            UpdateUI();
        }
    }

    public void IncreaseBet()
    {
        if (!gameInProgress)
        {
            currentBet += betIncrement;
            int maxBet = GetMaxBetForCurrentStat();

            if (currentBet > maxBet)
            {
                currentBet = maxBet;
            }

            UpdateUI();
        }
    }

    public void DecreaseBet()
    {
        if (!gameInProgress)
        {
            currentBet -= betIncrement;

            if (currentBet < minBet)
            {
                currentBet = minBet;
            }

            UpdateUI();
        }
    }

    // ========== GAME CONTROL ==========

    public void PlayGamble()
    {
        if (gameInProgress)
        {
            Debug.Log("Game already in progress!");
            return;
        }

        if (!playerData)
        {
            Debug.Log("PlayerData not assigned!");
            return;
        }

        if (CanAffordBet() && SpendStat())
        {
            gameInProgress = true;
            UpdateUI();

            if (gamblingPanel != null)
            {
                gamblingPanel.SetActive(true);
            }

            if (gamblingManager != null)
            {
                gamblingManager.StartNewGame(OnGameComplete);
            }
            else
            {
                Debug.LogError("GamblingManager not assigned!");
            }
        }
        else
        {
            Debug.Log($"Not enough {currentStatType}!");
        }
    }

    private bool CanAffordBet()
    {
        if (playerData == null) return false;

        switch (currentStatType)
        {
            case StatType.Money:
                return playerData.money >= currentBet;
            case StatType.HP:
                return playerData.hp >= currentBet;
            case StatType.Dmg:
                return playerData.damage >= currentBet;
            case StatType.Speed:
                return playerData.moveSpeed >= currentBet;
            default:
                return false;
        }
    }

    private bool SpendStat()
    {
        if (playerData == null) return false;

        switch (currentStatType)
        {
            case StatType.Money:
                playerData.money -= currentBet;
                return true;
            case StatType.HP:
                playerData.hp -= currentBet;
                return true;
            case StatType.Dmg:
                playerData.damage -= currentBet;
                return true;
            case StatType.Speed:
                playerData.moveSpeed -= currentBet;
                return true;
            default:
                return false;
        }
    }

    private void OnGameComplete(float multiplier)
    {
        int winAmount = Mathf.CeilToInt(currentBet * multiplier);
        AddWinToStat(winAmount);

        int profit = winAmount - currentBet;

        Debug.Log($"=== GAME RESULT ===");
        Debug.Log($"Bet: {currentBet}");
        Debug.Log($"Multiplier: {multiplier}x");
        Debug.Log($"Win: {winAmount}");
        Debug.Log($"Profit: {profit}");
        Debug.Log($"===================");

        gameInProgress = false;
        UpdateUI();
        playerStatsPanel.UpdateUI();

        Invoke(nameof(CloseGamblingPanel), 2f);
    }

    private void AddWinToStat(int amount)
    {
        if (playerData == null) return;

        switch (currentStatType)
        {
            case StatType.Money:
                playerData.money += amount;
                break;
            case StatType.HP:
                playerData.hp += amount;
                break;
            case StatType.Dmg:
                playerData.damage += amount;
                break;
            case StatType.Speed:
                playerData.moveSpeed += amount;
                break;
        }
    }

    public void CloseGamblingPanel()
    {
        if (gamblingPanel != null) gamblingPanel.SetActive(false);
        if (gamblingManager != null) gamblingManager.ResetGame();
    }

    // ========== UI ==========
    private void UpdateUI()
    {
        int maxBet = GetMaxBetForCurrentStat();

        if (betAmountText != null)
            betAmountText.text = $"Bet: {currentBet}";
        
        if (selectedStatText != null)
            selectedStatText.text = $"Betting: {currentStatType}";

        if (betSlider != null)
        {
            betSlider.maxValue = maxBet;

            if (!gameInProgress)
            {
                currentBet = Mathf.Clamp(currentBet, minBet, maxBet);
                if (!Mathf.Approximately(betSlider.value, currentBet))
                    betSlider.SetValueWithoutNotify(currentBet);
            }
        }
    }

    // ========== GETTERS ==========
    public StatType GetcurrentStatType() => currentStatType;
    public int GetCurrentBet() => currentBet;
    public bool IsGameInProgress() => gameInProgress;
}
