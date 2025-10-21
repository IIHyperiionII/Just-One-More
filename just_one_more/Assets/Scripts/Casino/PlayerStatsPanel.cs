using UnityEngine;
using TMPro;
using UnityEngine.PlayerLoop;

public class PlayerStatsPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI dmgText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI attackSpeedText;
    [SerializeField] private TextMeshProUGUI attackModifierText;

    private PlayerData playerData;
    // Changes based on whether the player is melee or ranged
    private string attackModifierName;

    void Start()
    {
        if (playerData != null) {
            attackModifierName = GetAttackModifierName();
            UpdateUI();
        }
    }

    public void SetPlayerData(PlayerData pd)
    {
        playerData = pd;
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

    public int GetAttackModifier()
    {
        if (playerData != null)
            return playerData.attackModifier;
        return 0;
    }

    public string GetAttackModifierName()
    {
        if (playerData != null)
        {
            return playerData.isMelee ? "Knockback" : "Bullet Speed";
        }
        return "Attack Modifier";
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

    public void SetAttackModifier(int value)
    {
        if (playerData != null)
        {
            playerData.attackModifier = value;
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

        UpdateTextField(hpText, "HP", playerData.hp);
        UpdateTextField(dmgText, "Damage", playerData.damage);
        UpdateTextField(speedText, "Speed", playerData.moveSpeed);
        UpdateTextField(moneyText, "Money", playerData.money);
        UpdateTextField(attackSpeedText, "Attack Speed", playerData.attackSpeed);
        UpdateTextField(attackModifierText, attackModifierName, playerData.attackModifier);
    }
}
