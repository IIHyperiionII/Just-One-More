using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class ShopManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button rerollButton;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemPriceText;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI rerollPriceText;

    [Header("Data")]
    [SerializeField] private PlayerStatsPanel playerStatsPanel;

    [Header("Shop sprites")]
    [SerializeField] private Sprite[] shopSprites;

    private PlayerData playerData;
    private ShopItem currentItem;
    private int previousRandomChoice = -1;
    public int rerollCount = 0;
    private int baseRerollPrice = 100;
    private int baseItemPrice = 100;
    private int itemLevelPriceIncrement = 150;

    void Start()
    {
    #if UNITY_EDITOR
        EnsureGameManagerForTesting();
    #endif

        if (GameManager.Instance != null)
        {
            playerData = GameManager.Instance.runtimePlayerData;
        }
        if (playerStatsPanel != null && playerData != null)
        {
            playerStatsPanel.SetPlayerData(playerData);
        }
        if (buyButton)
        {
            buyButton.onClick.AddListener(BuyCurrentItem);
        }
        if (rerollButton)
        {
            rerollButton.onClick.AddListener(RerollShop);
        }

        RerollShop();
    }

    #if UNITY_EDITOR
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

        Debug.Log("=== SHOP TEST SETUP COMPLETE ===");
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

    public void RerollShop()
    {
        if (rerollCount == 0)
        {
            rerollCount++;
        } 
        else
        {
            if (!playerStatsPanel.SpendMoney(baseRerollPrice * rerollCount))
            {
                Debug.Log("Not enough money for reroll.");
                return;
            } 

            rerollCount++;   
        }

        currentItem = GenerateRandomUpgrade();
        UpdateShopUI();
        UpdateRerollPrice();
    }

    private ShopItem GenerateRandomUpgrade()
    {
        int randomChoice;

        do
        {
            randomChoice = Random.Range(0, 5);
        
        } while ((randomChoice == previousRandomChoice && previousRandomChoice != 4) || LvlCheck(randomChoice));
        
        previousRandomChoice = randomChoice;

        switch (randomChoice)
        {
            case 0:
                return new ShopItem("Piercing", StatType.PiercingLevel);
            case 1:
                return new ShopItem("Dash", StatType.DashLevel);
            case 2:
                return new ShopItem("Block", StatType.BlockLevel);
            case 3:
                return new ShopItem("Freeze", StatType.FreezeLevel);
            case 4:
                return new ShopItem("Save Slot", StatType.SaveSlots);
            default:
                return new ShopItem("Save Slot", StatType.SaveSlots);
        }
    }

    private bool LvlCheck( int randomChoice)
    {
        switch (randomChoice)
        {
            case 0:
                return playerData.piercingLevel >= 4;
            case 1:
                return playerData.dashLevel >= 4;
            case 2:
                return playerData.blockLevel >= 4;
            case 3:
                return playerData.freezeLevel >= 4;
            case 4:
                return false;
            default:
                return false;
        }
    }

    private void BuyCurrentItem()
    {
        int currentLevel = GetCurrentValue(currentItem.statType);
        int price = CalculatePrice(currentLevel);

        if (playerStatsPanel.SpendMoney(price))
        {
            ApplyUpgrade(currentItem);

            rerollCount = 0;
            RerollShop();
        }
        else
        {
            Debug.Log("Not enough money.");
        }
    }

    private int CalculatePrice(int currentLevel)
    {
        return baseItemPrice + (currentLevel * itemLevelPriceIncrement);
    }

    private void ApplyUpgrade(ShopItem item)
    {
        if (playerData == null)
        {
            Debug.LogError("PlayerData is null!");
            return;
        }

        switch (item.statType)
        {
            case StatType.PiercingLevel:
                playerData.piercingLevel += 1;
                break;
            case StatType.DashLevel:
                playerData.dashLevel += 1;  
                break;
            case StatType.BlockLevel:
                playerData.blockLevel += 1;
                break;
            case StatType.FreezeLevel:
                playerData.freezeLevel += 1;
                break;
            case StatType.SaveSlots:
                playerData.numberOfSaves += 1;
                break;
        }

        playerStatsPanel.UpdateUI();
    }

    private int GetCurrentValue(StatType statType)
    {
        if (playerData == null) return 0;

        switch (statType)
        {
            case StatType.PiercingLevel:
                return playerData.piercingLevel;
            case StatType.DashLevel:
                return playerData.dashLevel;
            case StatType.BlockLevel:
                return playerData.blockLevel;
            case StatType.FreezeLevel:
                return playerData.freezeLevel;
            case StatType.SaveSlots:
                return playerData.numberOfSaves;
            default:
                return 0;
        }
    }

    private void UpdateShopUI()
    {
        int currentLevel = GetCurrentValue(currentItem.statType);
        int price = CalculatePrice(currentLevel);

        if (itemNameText)
        {
            itemNameText.text = currentItem.name +  $" {currentLevel} → {currentLevel + 1}";
        }

        if (itemPriceText)
        {
            itemPriceText.text = $"{price} $";
        }

        switch (currentItem.statType)
        {
            case StatType.PiercingLevel:
                itemIcon.sprite = shopSprites[0];
                break;
            case StatType.DashLevel:
                itemIcon.sprite = shopSprites[1];
                break;
            case StatType.BlockLevel:
                itemIcon.sprite = shopSprites[2];
                break;
            case StatType.FreezeLevel:
                itemIcon.sprite = shopSprites[3];
                break;
            case StatType.SaveSlots:
                itemIcon.sprite = shopSprites[4];
                break;
        }
    }
    
    private void UpdateRerollPrice()
    {
        if (rerollPriceText)
        {
            int nextRerollPrice = baseRerollPrice * rerollCount;
            rerollPriceText.text = $"{nextRerollPrice} $";
        }
    }

}

public struct ShopItem
{
    public string name;
    public StatType statType;


    public ShopItem(string name, StatType statType)
    {
        this.name = name;
        this.statType = statType;
    }
}
