using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

// Manages casino logic - betting, buttons for opening shop/minigames...

public class CasinoManager : MonoBehaviour
{
    // ========== FIELDS ==========

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI betAmountText;
    [SerializeField] private TextMeshProUGUI selectedStatText;
    [SerializeField] private Slider betSlider;
    [SerializeField] private TextMeshProUGUI remainingGamblesText;

    [Header("Stat Selection Buttons")]
    [SerializeField] private Button moneyButton;
    [SerializeField] private Button hpButton;
    [SerializeField] private Button damageButton;
    [SerializeField] private Button moveSpeedButton;
    [SerializeField] private Button attackSpeedButton;
    [SerializeField] private Button attackModifierButton;
    [SerializeField] private Image attackModifierButtonImage;
    [SerializeField] private Sprite bulletSpeedSprite;
    [SerializeField] private Sprite knockbackSprite;

    [Header("Panels")]
    [SerializeField] private GameObject casinoPanel;
    [SerializeField] private GameObject plinkoPanel;
    [SerializeField] private GameObject blackjackPanel;

    [Header("Data")]
    [SerializeField] private PlayerStatsPanel playerStatsPanel;

    [Header("Game References")]
    [SerializeField] private PlinkoManager plinkoManager;
    [SerializeField] private BlackjackManager blackjackManager;
    [SerializeField] private ShopManager shopManager;

    private int minBet = 1;
    private PlayerData playerData;
    private int currentBet;
    private StatType currentStatType = StatType.Money;
    private bool gambleGameInProgress = false;
    private Color selectedColor = Color.green;
    private Color normalColor = new Color(0f, 0.65f, 0.7f, 1f);
    private StatType attackModifierStatType = StatType.BulletSpeed;
    private int remainingGambles = 5;
    private float minigameStartTime;
    private const float MIN_MINIGAME_DURATION = 30f;
    private int needToGambleReduction = 15;
    void Start()
    {
#if UNITY_EDITOR
        // Testing helper - creates GameManager if missing when running Casino scene directly
        EnsureGameManagerForTesting();
#endif

        // Load player data from persistent GameManager
        if (GameManager.Instance != null && GameManager.Instance.runtimePlayerData != null){
            playerData = GameManager.Instance.runtimePlayerData;
        }
        
        if (playerStatsPanel && playerData != null)
        {
            playerStatsPanel.SetPlayerData(playerData);
            
            // Determine which attack modifier is active (Bullet Speed or Knockback)
            attackModifierStatType = playerStatsPanel.GetAttackModifierStatType();

            if (attackModifierButtonImage)
            {
                if (playerStatsPanel.GetAttackModifierName() == "Bullet Speed")
                    attackModifierButtonImage.sprite = bulletSpeedSprite;
                else
                    attackModifierButtonImage.sprite = knockbackSprite;
            }
        }

        if (plinkoPanel)
        {
            plinkoPanel.SetActive(false);
        }

        if (blackjackPanel)
        {
            blackjackPanel.SetActive(false);
        }

        currentBet = minBet;

        SetupSlider();
        UpdateStatButtons();
        UpdateUI();
    }

    // ========== INITIALIZATION ==========

#if UNITY_EDITOR
    // Testing helper: Sets up minimal scene requirements for Casino testing
    // Safe to keep - only compiles in Editor, not in builds
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
        
            // Disable GameManager to prevent it from running game loop
            // We only need its data container (PlayerData), not game logic
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

        // Create dummy scene objects that GameManager expects to exist
        CreateDummyIfMissing("Player", "DummyPlayer");
        CreateDummyIfMissing("Background", "DummyBackground", addSprite: true);
        CreateDummyIfMissing("Background2", "DummyBackground2", addSprite: true, setInactive: true);
        CreateDummyIfMissing("Background3", "DummyBackground3", addSprite: true, setInactive: true);
        CreateDummyIfMissing("EnemiesParent", "EnemiesParent");
        CreateDummyIfMissing("BoundsCheckDoors", "DummyBoundsCheckDoors", addCollider: true);
        CreateDummyIfMissing("BoundsCheckPlayer", "DummyBoundsCheckPlayer", addCollider: true);

        // Ensure main camera has correct tag
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

    void OnEnable()
    {
        // Reset state each time casino panel is opened
        remainingGambles = 5;
        
        if (shopManager != null)
        {
            shopManager.rerollCount = 0;
        }
        
         if (playerStatsPanel != null){
            // Refresh player data in case it changed
            if (GameManager.Instance != null && GameManager.Instance.runtimePlayerData != null) {
                playerData = GameManager.Instance.runtimePlayerData;
            }
            playerStatsPanel.SetPlayerData(playerData);
            playerStatsPanel.UpdateUI();
        }

        
        UpdateUI();
    }

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
                playerData.money = Mathf.Max(0, playerData.money + amount);
                // In MoneyLife mode, HP mirrors Money value
                if (ModeController.Instance.currentSelection.selectedMode == GameMode.MoneyLife)
                {
                    playerData.hp = playerData.money;
                }
                break;
            case StatType.HP:
                playerData.hp = Mathf.Max(0, playerData.hp + amount);
                // Mark player as dead if HP drops to 0
                if (playerData.hp <= 0) playerData.isDead = true;
                // In MoneyLife mode, Money mirrors HP value
                if (ModeController.Instance.currentSelection.selectedMode == GameMode.MoneyLife)
                {
                    playerData.money = playerData.hp;
                }
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
        if (remainingGambles == 0)
            return;

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
            return;
        }

        if (SpendStat())
        {
            gambleGameInProgress = true;
            minigameStartTime = Time.time;
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
        if (remainingGambles == 0)
            return;

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
            return;
        }

        if (SpendStat())
        {
            gambleGameInProgress = true;
            minigameStartTime = Time.time;
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
                Debug.LogWarning("blackjackManager not assigned!");
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
        remainingGambles--;
        // Reduce gambling addiction counter (forces player to gamble less)
        playerData.needToGamble = Mathf.Max(0, playerData.needToGamble -= needToGambleReduction);

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

    // Called by EXIT button - allows closing game early with bet refund under specific conditions
    public void ForceCloseGamblePanel()
    {
        bool canForceClose;

        if (gambleGameInProgress)
        {
            float timeSinceStart = Time.time - minigameStartTime;

            // Prevent abuse: only allow force close if conditions met

            // 1. Minimum time elapsed (prevents instant quit)
            bool timeCondition = timeSinceStart >= MIN_MINIGAME_DURATION;

            // 2. Ball is stuck (Plinko-specific failsafe)
            bool ballCondition = false;
            if (plinkoPanel != null && plinkoPanel.activeSelf && plinkoManager != null)
                ballCondition = plinkoManager.IsBallStuck();

            canForceClose = timeCondition || ballCondition;

            if (!canForceClose) {
                Debug.Log($"Cannot force close yet. Time elapsed: {timeSinceStart:F1}s / {MIN_MINIGAME_DURATION}s");
                return;
            }
        }

        // Refund bet to player if game was in progress
        if (gambleGameInProgress && playerData != null)
        {
            AddWinToStat(currentBet);
            remainingGambles++;
            Debug.LogWarning("Game force closed - bet returned to player");
        }

        gambleGameInProgress = false;

        if (plinkoPanel)
            plinkoPanel.SetActive(false);

        if (plinkoManager)
            plinkoManager.ResetGame();

        if (blackjackPanel)
            blackjackPanel.SetActive(false);
        
        if (blackjackManager)
            blackjackManager.ResetGame();

        UpdateUI();
        playerStatsPanel.UpdateUI();
    }

    // ========== UI ==========

    private void UpdateUI()
    {
        int maxBet = GetMaxBetForCurrentStat();

        if (betAmountText)
            betAmountText.text = $"Bet: {currentBet}";

        if (remainingGamblesText)
            remainingGamblesText.text = $"Remaining gambles: {remainingGambles}";

        switch (currentStatType)
        {
            case StatType.Money:
                selectedStatText.text = "Betting: Money";
                break;
            case StatType.HP:
                selectedStatText.text = "Betting: HP";
                break;
            case StatType.Dmg:
                selectedStatText.text = "Betting: Dmg";
                break;
            case StatType.MoveSpeed:
                selectedStatText.text = "Betting: Move Speed";
                break;
            case StatType.AttackSpeed:
                selectedStatText.text = "Betting: Attack Speed";
                break;
            case StatType.BulletSpeed:
                selectedStatText.text = $"Betting: Bullet Speed";
                break;
            case StatType.Knockback:
                selectedStatText.text = $"Betting: Knockback";
                break;
            default:
                selectedStatText.text = $"Betting: {currentStatType}";
                break;
        }

        if (betSlider)
        {
            // Disable slider if player doesn't have enough of current stat
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
                    // Clamp bet to valid range
                    currentBet = Mathf.Clamp(currentBet, minBet, maxBet);
                    // Sync slider without triggering OnValueChanged listener
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
            // Highlight selected button (green vs cyan)
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
