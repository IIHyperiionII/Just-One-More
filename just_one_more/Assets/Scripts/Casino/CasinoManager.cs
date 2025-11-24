using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class CasinoManager : MonoBehaviour
{
    // ========== FIELDS ==========

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
    [SerializeField] private Button attackModifierButton;
    [SerializeField] private TextMeshProUGUI attackModifierButtonText;

    [Header("Panels")]
    [SerializeField] private GameObject casinoPanel;
    [SerializeField] private GameObject plinkoPanel;
    [SerializeField] private GameObject blackjackPanel;

    [Header("Data")]
    [SerializeField] private PlayerStatsPanel playerStatsPanel;

    [Header("Game References")]
    [SerializeField] private PlinkoManager plinkoManager;
    [SerializeField] private BlackjackManager blackjackManager;

    private int minBet = 1;
    private PlayerData playerData;
    private int currentBet;
    private StatType currentStatType = StatType.Money;
    private bool gambleGameInProgress = false;
    private Color selectedColor = Color.green;
    private Color normalColor = Color.white;
    private string attackModifierName;
    private StatType attackModifierStatType = StatType.BulletSpeed;

    void Start()
    {
#if UNITY_EDITOR
        /*
            EnsureGameManagerForTesting:
            ONLY FOR TESTING
            REMOVE BEFORE MERGE
            +
            REMOVE RESOURCES FROM UNITY ASSETS IN PROJECT
        */
        EnsureGameManagerForTesting();
#endif

        if (GameManager.Instance != null && GameManager.Instance.runtimePlayerData != null)
            // Get the runtime PlayerData from GameManager
            playerData = GameManager.Instance.runtimePlayerData;

        if (playerStatsPanel && playerData != null)
        {
            playerStatsPanel.SetPlayerData(playerData);
            attackModifierName = playerStatsPanel.GetAttackModifierName();
            attackModifierStatType = playerStatsPanel.GetAttackModifierStatType();

            if (attackModifierButtonText)
            {
                attackModifierButtonText.text = attackModifierName;
            }
        }

        if (plinkoPanel)
        {
            plinkoPanel.SetActive(false);
        }

        currentBet = minBet;

        SetupSlider();
        UpdateStatButtons();
        UpdateUI();
    }

    // ========== INITIALIZATION ==========

#if UNITY_EDITOR
    // Create a minimal GameManager and runtime PlayerData when none exists (testing only).
    // Remove this helper for production builds.
    private void EnsureGameManagerForTesting()
    {
        if (GameManager.Instance != null) return;

        var existing = FindFirstObjectByType<GameManager>();
        if (existing != null) return;

        // Load prefab from Resources/GameManager.prefab
        var prefab = Resources.Load<GameObject>("GameManager");
        if (prefab != null)
        {
            GameObject gmObject = Instantiate(prefab);
        
            // Explicitně vypnout GameManager komponentu
            GameManager gm = gmObject.GetComponent<GameManager>();
            if (gm != null)
            {
                gm.enabled = false;
                Debug.Log("GameManager instantiated and DISABLED.");
            }
        }
        else
        {
            Debug.LogWarning("GameManager prefab not found in Resources folder!");
        }

        // Create dummy objects
        CreateDummyIfMissing("Player", "DummyPlayer");
        CreateDummyIfMissing("Background", "DummyBackground", addSprite: true);
        CreateDummyIfMissing("Background2", "DummyBackground2", addSprite: true, setInactive: true);
        CreateDummyIfMissing("Background3", "DummyBackground3", addSprite: true, setInactive: true);
        CreateDummyIfMissing("EnemiesParent", "EnemiesParent");
        CreateDummyIfMissing("BoundsCheckDoors", "DummyBoundsCheckDoors", addCollider: true);
        CreateDummyIfMissing("BoundsCheckPlayer", "DummyBoundsCheckPlayer", addCollider: true);

        // Set main camera tag
        if (Camera.main != null && Camera.main.tag != "MainCamera")
            Camera.main.tag = "MainCamera";

        Debug.Log("=== CASINO TEST SETUP COMPLETE ===");
    }

    private void CreateDummyIfMissing(string tag, string name, bool addSprite = false, 
                                   bool setInactive = false, bool addCollider = false)
    {
        if (GameObject.FindGameObjectWithTag(tag) == null)
        {
            GameObject dummy = new GameObject(name);
            dummy.tag = tag;
            if (addSprite) dummy.AddComponent<SpriteRenderer>();
            if (addCollider)
            {
                BoxCollider2D col = dummy.AddComponent<BoxCollider2D>();
                col.isTrigger = true;
            }
            if (setInactive) dummy.SetActive(false);
        }
    }
#endif
    
    private void SetupSlider()
    {
        if (betSlider)
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

    public void SelectAttackModifier()
    {
        SelectStat(attackModifierStatType);
    }
    
    private void SelectStat(StatType statType)
    {
        if (!gambleGameInProgress)
        {
            currentStatType = statType;
            currentBet = minBet;
            
            if (betSlider)
            {
                betSlider.maxValue = GetMaxBetForCurrentStat();
                betSlider.SetValueWithoutNotify(currentBet);
            }
            
            UpdateStatButtons();
            UpdateUI();
        }
    }

    // ========== BETTING LOGIC ==========

    private int GetMaxBetForCurrentStat()
    {
        if (!playerData) return minBet;

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
            case StatType.Knockback:
                return playerData.knockback;
            case StatType.BulletSpeed:
                return playerData.bulletSpeed;
            default:

                return minBet;
        }
    }

    private bool CanAffordBet()
    {
        if (playerData == null) return false;

        int maxBet = GetMaxBetForCurrentStat();

        return maxBet >= minBet && currentBet >= minBet && currentBet <= maxBet;
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
            case StatType.Knockback:
                playerData.knockback -= currentBet;
                return true;
            default:
                return false;
        }
    }

    private void AddWinToStat(int amount)
    {
        if (playerData == null) return;

        switch (currentStatType)
        {
            case StatType.Money:
                // TODO: (playerData.money + amount) > max ? max : playerData.money + amount
                playerData.money = Mathf.Max(0, playerData.money + amount);
                break;
            case StatType.HP:
                // HP == 0 means player's death (TODO: achievement)
                playerData.hp = Mathf.Max(0, playerData.hp + amount);
                if (playerData.hp <= 0) playerData.isDead = true;
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
            case StatType.Knockback:
                playerData.knockback = Mathf.Max(1, playerData.knockback + amount);
                break;
        }
    }

    private void OnSliderChanged(float value)
    {
        if (!gambleGameInProgress)
        {
            currentBet = Mathf.RoundToInt(value);
            UpdateUI();
        }
    }

    // ========== GAME CONTROL ==========

    public void StartPlinko()
    {
        if (gambleGameInProgress)
        {
            Debug.LogWarning("Game already in progress!");
            return;
        }

        if (!playerData)
        {
            Debug.LogError("PlayerData not assigned!");
            return;
        }

        if (!CanAffordBet())
        {
            // Player doesn't have enough stats
            // TODO: UI feedback
            return;
        }

        if (SpendStat())
        {
            gambleGameInProgress = true;
            UpdateUI();

            if (plinkoPanel)
                plinkoPanel.SetActive(true);

            if (plinkoManager)
            {
                // Give callback function OnGameComplete to be called when game ends
                plinkoManager.StartNewGame(OnGameComplete);
            }
            else
            {
                Debug.LogWarning("plinkoManager not assigned!");
            }
        }
    }

    public void StartBlackjack()
    {
         if (gambleGameInProgress)
        {
            Debug.LogWarning("Game already in progress!");
            return;
        }

        if (!playerData)
        {
            Debug.LogError("PlayerData not assigned!");
            return;
        }

        if (!CanAffordBet())
        {
            // Player doesn't have enough stats
            // TODO: UI feedback
            return;
        }

        if (SpendStat())
        {
            gambleGameInProgress = true;
            UpdateUI();

            if (blackjackPanel)
                blackjackPanel.SetActive(true);

            if (blackjackManager)
            {
                // Give callback function OnGameComplete to be called when game ends
                blackjackManager.StartNewGame(OnGameComplete);
            }
            else
            {
                Debug.LogWarning("plinkoManager not assigned!");
            }
        }
    }

    private void OnGameComplete(float multiplier)
    {
        int winAmount = Mathf.CeilToInt(currentBet * multiplier);
        AddWinToStat(winAmount);

        int profit = winAmount - currentBet;

#if UNITY_EDITOR
        Debug.Log($"=== GAME RESULT ===");
        Debug.Log($"Bet: {currentBet}");
        Debug.Log($"Multiplier: {multiplier}x");
        Debug.Log($"Profit: {profit}");
        Debug.Log($"===================");
#endif

        gambleGameInProgress = false;
        UpdateUI();
        playerStatsPanel.UpdateUI();

        // Close gambling panel after 2 seconds
        StartCoroutine(CloseAfterDelay(2f));
    }

    IEnumerator CloseAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay); // ignores Time.timeScale
        CloseGamblePanel();
    }


    public void CloseGamblePanel()
    {
        if (!gambleGameInProgress)
        {
            if (plinkoPanel) plinkoPanel.SetActive(false);
            if (plinkoManager) plinkoManager.ResetGame();
            if (blackjackPanel) blackjackPanel.SetActive(false);
            if (blackjackManager) blackjackManager.ResetGame();
        }
    }

    // Close for EXIT button in gambling panel
    // PROBLEM - TODO: Player can now rig the game by exiting before a bad outcome
    public void ForceCloseGamblePanel()
    {
        // Return bet to player
        if (gambleGameInProgress && playerData != null)
        {
            AddWinToStat(currentBet);
            Debug.LogWarning("Game force closed - bet returned to player");
        }

        gambleGameInProgress = false;

        if (plinkoPanel)
            plinkoPanel.SetActive(false);

        if (plinkoManager)
            plinkoManager.ResetGame();

        UpdateUI();
        playerStatsPanel.UpdateUI();
    }

    // ========== UI ==========

    private void UpdateUI()
    {
        int maxBet = GetMaxBetForCurrentStat();

        if (betAmountText)
            betAmountText.text = $"Bet: {currentBet}";

        if (selectedStatText)
        {
            if (currentStatType == attackModifierStatType)
            {
                selectedStatText.text = $"Betting: {attackModifierName}";
            }
            else
            {
                selectedStatText.text = $"Betting: {currentStatType}";
            }

        }

        if (betSlider)
        {
            // If current value of statType is less than minBet (Only for money for now), disable the slider
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
                betSlider.interactable = !gambleGameInProgress;

                if (!gambleGameInProgress)
                {
                    // Returns currentBet if its between min and max, else returns min or max
                    currentBet = Mathf.Clamp(currentBet, minBet, maxBet);
                    // Update slider without invoking OnValueChanged event if slider value != current bet
                    if (!Mathf.Approximately(betSlider.value, currentBet))
                        betSlider.SetValueWithoutNotify(currentBet);
                }
            }
        }
    }

    private void UpdateStatButtons()
    {
        SetButtonColor(moneyButton, currentStatType == StatType.Money);
        SetButtonColor(hpButton, currentStatType == StatType.HP);
        SetButtonColor(damageButton, currentStatType == StatType.Dmg);
        SetButtonColor(moveSpeedButton, currentStatType == StatType.MoveSpeed);
        SetButtonColor(attackSpeedButton, currentStatType == StatType.AttackSpeed);
        SetButtonColor(attackModifierButton, currentStatType == attackModifierStatType);
    }
    
     private void SetButtonColor(Button button, bool isSelected)
    {
        if (button)
        {
            // Selected button is green, others are white
            ColorBlock colors = button.colors;
            colors.normalColor = isSelected ? selectedColor : normalColor;
            colors.selectedColor = isSelected ? selectedColor : normalColor;
            button.colors = colors;
        }
    }

    // ========== GETTERS ==========
    public StatType GetCurrentStatType() => currentStatType;
    public int GetCurrentBet() => currentBet;
    public bool IsGambleGameInProgress() => gambleGameInProgress;
}
