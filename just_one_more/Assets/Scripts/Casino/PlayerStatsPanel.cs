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
    [SerializeField] private TextMeshProUGUI bulletSpeedText;

    private PlayerData playerData;
    void Start()
    {
        if (playerData != null)
            UpdateUI();
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

    public void SetMoney(int amount)
    {
        if (playerData != null)
        {
            playerData.money = amount;
            UpdateUI();
        }
    }

    // ========== OTHER STATS METHODS ==========
    
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

    // renamed to GetMoveSpeed and returns float
    public float GetMoveSpeed()
    {
        if (playerData != null)
            return playerData.moveSpeed;
        return 0;
    }

    // attackSpeed / bulletSpeed getters
    public int GetAttackSpeed()
    {
        if (playerData != null)
            return playerData.attackSpeed;
        return 0;
    }

    public int GetBulletSpeed()
    {
        if (playerData != null)
            return playerData.bulletSpeed;
        return 0;
    }

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

    public void SetBulletSpeed(int value)
    {
        if (playerData != null)
        {
            playerData.bulletSpeed = value;
            UpdateUI();
        }
    }

    // ========== UI UPDATE ==========
    
    public void UpdateUI()
    {
        if (playerData == null)
        {
            Debug.Log("PlayerData not assigned to PlayerStatsPanel!");
            return;
        }

        if (hpText != null)
            hpText.text = $"HP: {playerData.hp}";
        
        if (dmgText != null)
            dmgText.text = $"DMG: {playerData.damage}";
        
        if (speedText != null)
            speedText.text = $"Move Speed: {playerData.moveSpeed}";
        
        if (moneyText != null)
            moneyText.text = $"Money: {playerData.money}";

        if (attackSpeedText != null)
            attackSpeedText.text = $"Attack Speed: {playerData.attackSpeed}";

        if (bulletSpeedText != null)
            bulletSpeedText.text = $"Bullet Speed: {playerData.bulletSpeed}";
    }
}
