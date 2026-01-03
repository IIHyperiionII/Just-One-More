using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Panel in casino showing all players stats

public class PlayerStatsPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI dmgText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI attackSpeedText;
    [SerializeField] private TextMeshProUGUI attackModifierText;
    [SerializeField] private TextMeshProUGUI piercingText;
    [SerializeField] private TextMeshProUGUI dashText;
    [SerializeField] private TextMeshProUGUI blockText;
    [SerializeField] private TextMeshProUGUI freezeText;
    [SerializeField] private TextMeshProUGUI saveSlotText;
    [SerializeField] private Image attackModifierImage;
    [SerializeField] private Sprite bulletSpeedSprite;
    [SerializeField] private Sprite knockbackSprite;

    private PlayerData playerData;
    // Cached name: "Knockback" for melee, "Bullet Speed" for ranged
    private string attackModifierName;

    void Start()
    {
        // PlayerData is typically set via SetPlayerData() from parent (CasinoManager)
        if (playerData != null) 
        {
            attackModifierName = GetAttackModifierName();
            UpdateUI();
        }
    }

    public void SetPlayerData(PlayerData pd)
    {
        playerData = pd;
        if (playerData != null) {
            // Cache modifier name based on weapon type (melee vs ranged)
            attackModifierName = GetAttackModifierName();
        }
        UpdateUI();
    }

    public PlayerData GetPlayerData() => playerData;

    // ========== MONEY METHODS ==========
    public int GetMoney()
    {
        if (playerData != null)
            return playerData.money;
        return 0;
    }

    public void SetMoney(int amount)
    {
        if (playerData != null)
        {
            playerData.money = amount;
            UpdateUI();
        }
    }

    public bool SpendMoney(int amount)
    {
        if (playerData != null && playerData.money >= amount)
        {
            playerData.money -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    public void AddMoney(int amount)
    {
        if (playerData != null)
        {
            playerData.money += amount;
            UpdateUI();
        }
    }

    // ========== GETTERS ==========
    
    public int GetHP()
    {
        if (playerData != null)
            return playerData.hp;
        return 0;
    }

    public int GetDMG()
    {
        if (playerData != null)
            return playerData.damage;
        return 0;
    }

    public int GetMoveSpeed()
    {
        if (playerData != null)
            return playerData.moveSpeed;
        return 0;
    }

    public int GetAttackSpeed()
    {
        if (playerData != null)
            return playerData.attackSpeed;
        return 0;
    }

    // Returns current attack modifier value (Knockback for melee, Bullet Speed for ranged)
    public int GetAttackModifier()
    {
        if (playerData != null)
            return ModeController.Instance.currentSelection.selectedWeapon == WeaponType.Melee ? playerData.knockback : playerData.bulletSpeed;
        return 0;
    }

    // Returns display name of attack modifier based on weapon type
    public string GetAttackModifierName()
    {
        if (playerData != null)
        {
            return ModeController.Instance.currentSelection.selectedWeapon == WeaponType.Melee ? "Knockback" : "Bullet Speed";
        }
        return "Attack Modifier";
    }

    // Returns StatType enum for attack modifier (used for betting in casino)
    public StatType GetAttackModifierStatType()
    {
        if (playerData != null)
        {
            return ModeController.Instance.currentSelection.selectedWeapon == WeaponType.Melee ? StatType.Knockback : StatType.BulletSpeed;
        }
        return StatType.BulletSpeed; // Default
    }

    // ========== SETTERS ==========
    public void SetHP(int value)
    {
        if (playerData != null)
        {
            playerData.hp = value;
            UpdateUI();
        }
    }

    public void SetDMG(int value)
    {
        if (playerData != null)
        {
            playerData.damage = value;
            UpdateUI();
        }
    }

    public void SetMoveSpeed(int value)
    {
        if (playerData != null)
        {
            playerData.moveSpeed = value;
            UpdateUI();
        }
    }

    public void SetAttackSpeed(int value)
    {
        if (playerData != null)
        {
            playerData.attackSpeed = value;
            UpdateUI();
        }
    }

    // Sets attack modifier based on weapon type (knockback for melee, bulletSpeed for ranged)
    public void SetAttackModifier(int value)
    {
        if (playerData != null)
        {
            if (ModeController.Instance.currentSelection.selectedWeapon == WeaponType.Melee)
                playerData.knockback = value;
            else
                playerData.bulletSpeed = value;
            UpdateUI();
        }
    }

    // ========== UI UPDATE ==========

    private void UpdateTextField(TextMeshProUGUI textField, string label, int value)
    {
        if (textField)
        {
            textField.text = $"{label}: {value}";
        }
    }

    public void UpdateUI()
    {
        if (playerData == null)
        {
            Debug.LogError("PlayerData not assigned to PlayerStatsPanel!");
            return;
        }
        if (moneyText)
        {
            moneyText.text = $"Money: {playerData.money} $";
        }
        UpdateTextField(hpText, "HP", playerData.hp);
        UpdateTextField(dmgText, "Damage", playerData.damage);
        UpdateTextField(speedText, "Move Speed", playerData.moveSpeed);
        UpdateTextField(attackSpeedText, "Attack Speed", playerData.attackSpeed);
        
        // Display correct modifier based on weapon type (cached name, current value)
        int attackModifierValue = ModeController.Instance.currentSelection.selectedWeapon == WeaponType.Melee ? playerData.knockback : playerData.bulletSpeed;
        UpdateTextField(attackModifierText, attackModifierName, attackModifierValue);
        
        UpdateTextField(piercingText, "Piercing Level", playerData.piercingLevel);
        UpdateTextField(dashText, "Dash Level", playerData.dashLevel);
        UpdateTextField(blockText, "Block Level", playerData.blockLevel);
        UpdateTextField(freezeText, "Freeze Level", playerData.freezeLevel);
        UpdateTextField(saveSlotText, "Save slots", playerData.numberOfSaves);
    
        // Update icon to match current weapon type
        if (attackModifierImage)
            {
            if (GetAttackModifierName() == "Bullet Speed")
                attackModifierImage.sprite = bulletSpeedSprite;
            else
                attackModifierImage.sprite = knockbackSprite;
        }
    }
}
