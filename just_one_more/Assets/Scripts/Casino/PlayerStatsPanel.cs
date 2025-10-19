using UnityEngine;
using TMPro;

public class PlayerStatsPanel : MonoBehaviour
{
    [Header("Data Reference")]
    [SerializeField] private PlayerData playerData;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI dmgText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI moneyText;

    void Start()
    {
        UpdateUI();
    }

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

    public float GetSpeed()
    {
        if (playerData != null)
            return playerData.moveSpeed;
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

    public void SetSpeed(float value)
    {
        if (playerData != null)
        {
            playerData.moveSpeed = value;
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
            speedText.text = $"Speed: {playerData.moveSpeed:F1}";
        
        if (moneyText != null)
            moneyText.text = $"Money: {playerData.money}";
    }

    // ========== PUBLIC GETTER ==========
    
    public PlayerData GetPlayerData() => playerData;
}
