using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public enum StatType
{
    Money,
    HP,
    Dmg,
    MoveSpeed,
    AttackSpeed,
    BulletSpeed
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
    [SerializeField] private Button moveSpeedButton;
    [SerializeField] private Button attackSpeedButton;
    [SerializeField] private Button bulletSpeedButton;

    [Header("Panels")]
    [SerializeField] private GameObject casinoPanel;
    [SerializeField] private GameObject gamblingPanel;

    [Header("Data")]
    [SerializeField] private PlayerStatsPanel playerStatsPanel;

    [Header("Game References")]
    [SerializeField] private GamblingManager gamblingManager;

    private int minBet = 1;
    private PlayerData playerData;
    private int currentBet;
    private StatType currentStatType = StatType.Money;
    private bool gameInProgress = false;
    private Color selectedColor = Color.green;
    private Color normalColor = Color.white;

    void Start()
    {
        /*
            ONLY FOR TESTING

            REMOVE BEFORE MERGE
            
            +

            REMOVE RESOURCES FROM UNITY ASSETS IN PROJECT
        */
        EnsureGameManagerForTesting();

        if (GameManager.Instance != null && GameManager.Instance.runtimePlayerData != null)
            playerData = GameManager.Instance.runtimePlayerData;

        if (playerStatsPanel != null && playerData != null)
            playerStatsPanel.SetPlayerData(playerData);

        currentBet = minBet;

        SetupSlider();

        if (gamblingPanel != null)
        {
            gamblingPanel.SetActive(false);
        }

        UpdateStatButtonColors();
        UpdateUI();
    }

    // Create a minimal GameManager and runtime PlayerData when none exists (testing only).
    // Remove this helper for production builds.
    private void EnsureGameManagerForTesting()
{
    if (GameManager.Instance != null) return;

    var existing = FindFirstObjectByType<GameManager>();
    if (existing != null) return;

    // načti prefab z Resources/GameManager.prefab
    var prefab = Resources.Load<GameObject>("GameManager");
    if (prefab != null)
    {
        Instantiate(prefab); // Awake se zavolá automaticky, basePlayerData už je přiřazeno
        Debug.Log("GameManager prefab instantiated from Resources.");
    }
    else
    {
        Debug.LogError("GameManager prefab not found in Resources folder!");
    }
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

    public void SelectMoveSpeed()
    {
        SelectStat(StatType.MoveSpeed);
    }

    public void SelectAttackSpeed()
    {
        SelectStat(StatType.AttackSpeed);
    }

    public void SelectBulletSpeed()
    {
        SelectStat(StatType.BulletSpeed);
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
        SetButtonColor(moveSpeedButton, currentStatType == StatType.MoveSpeed);
        SetButtonColor(attackSpeedButton, currentStatType == StatType.AttackSpeed);
        SetButtonColor(bulletSpeedButton, currentStatType == StatType.BulletSpeed);
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
            case StatType.MoveSpeed:
                return playerData.moveSpeed;
            case StatType.AttackSpeed:
                return playerData.attackSpeed;
            case StatType.BulletSpeed:
                return playerData.bulletSpeed;
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

        // int maxBet = GetMaxBetForCurrentStat();

        // return maxBet >= minBet && currentBet >= minBet && currentBet <= maxBet;
        switch (currentStatType)
        {
            case StatType.Money:
                return playerData.money > 0 && playerData.money >= currentBet;
            case StatType.HP:
                return playerData.hp > 0 && playerData.hp >= currentBet;
            case StatType.Dmg:
                return playerData.damage > 0 && playerData.damage >= currentBet;
            case StatType.MoveSpeed:
                return playerData.moveSpeed > 0 && playerData.moveSpeed >= currentBet;
            case StatType.AttackSpeed:
                return playerData.attackSpeed > 0 && playerData.attackSpeed >= currentBet;
            case StatType.BulletSpeed:
                return playerData.bulletSpeed > 0 && playerData.bulletSpeed >= currentBet;
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
            case StatType.MoveSpeed:
                playerData.moveSpeed -= currentBet;
                return true;
            case StatType.AttackSpeed:
                playerData.attackSpeed -= currentBet;
                return true;
            case StatType.BulletSpeed:
                playerData.bulletSpeed -= currentBet;
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
                playerData.money = Mathf.Max(0, playerData.money + amount);
                break;
            case StatType.HP:
                playerData.hp = Mathf.Max(0, playerData.hp + amount); // HP nesmí být 0
                break;
            case StatType.Dmg:
                playerData.damage = Mathf.Max(1, playerData.damage + amount);
                break;
            case StatType.MoveSpeed:
                playerData.moveSpeed = Mathf.Max(1, playerData.moveSpeed + amount);
                break;
            case StatType.AttackSpeed:
                playerData.attackSpeed = Mathf.Max(1, playerData.attackSpeed + amount);
                break;
            case StatType.BulletSpeed:
                playerData.bulletSpeed = Mathf.Max(1, playerData.bulletSpeed + amount);
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
            if (maxBet < minBet)
            {
                betSlider.minValue = 0;
                betSlider.maxValue = 0;
                betSlider.interactable = false;
                betSlider.SetValueWithoutNotify(0);
            }
            else
            {
                betSlider.minValue = minBet;
                betSlider.maxValue = maxBet;
                betSlider.interactable = !gameInProgress;

                if (!gameInProgress)
                {
                    currentBet = Mathf.Clamp(currentBet, minBet, maxBet);
                    if (!Mathf.Approximately(betSlider.value, currentBet))
                        betSlider.SetValueWithoutNotify(currentBet);
                }
            }
        }
    }

    // ========== GETTERS ==========
    public StatType GetcurrentStatType() => currentStatType;
    public int GetCurrentBet() => currentBet;
    public bool IsGameInProgress() => gameInProgress;
}
