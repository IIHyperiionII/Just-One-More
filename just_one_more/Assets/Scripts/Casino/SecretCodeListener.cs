using UnityEngine;
using Unity;

// Listener in casino for secret buff codes (JJK references)

public class SecretCodeListener : MonoBehaviour
{
    private string currentInput = "";
    private int maxBufferLength = 8; // Longest code is "mahoraga" (8 chars)
    private GameModeManager gameModeManager;
    
    void Start()
    {
        gameModeManager = FindFirstObjectByType<GameModeManager>();
        
        if (gameModeManager == null)
        {
            Debug.LogError("GameModeManager not found!");
        }
    }
    
    void Update()
    {
        // Only listen for codes during minigames
        if (gameModeManager == null || !gameModeManager.inMiniGame) return;
        
        if (Input.anyKeyDown)
        {
            foreach (char c in Input.inputString)
            {
                // Add typed character to buffer (lowercase for easier matching)
                currentInput += c.ToString().ToLower();
                
                // Keep only last N characters (sliding window)
                if (currentInput.Length > maxBufferLength)
                {
                    currentInput = currentInput.Substring(1);
                }
                
                CheckAllCodes();
            }
        }
    }
    
    void CheckAllCodes()
    {
        // "mahoraga" = mega buff (tank build)
        if (currentInput.EndsWith("mahoraga"))
        {
            ActivateMahoraga();
            currentInput = "";
        }
        // "gojo" = ultimate buff (max everything)
        else if (currentInput.EndsWith("gojo"))
        {
            ActivateGojo();
            currentInput = "";
        }
        // "yuta" or "megumi" = debuff (reset to minimum)
        else if (currentInput.EndsWith("yuta") || currentInput.EndsWith("megumi"))
        {
            ActivateBums();
            currentInput = "";
        }
    }

    // Mahoraga buff: Eight-Handled Sword Divergent Sila Divine General
    void ActivateMahoraga()
    {
        if (GameManager.Instance != null && GameManager.Instance.runtimePlayerData != null)
        {
            GameManager.Instance.runtimePlayerData.money += 999999;
            GameManager.Instance.runtimePlayerData.hp += 99999;
            GameManager.Instance.runtimePlayerData.damage += 999;
            GameManager.Instance.runtimePlayerData.moveSpeed += 99;
            GameManager.Instance.runtimePlayerData.numberOfSaves = 100;
            GameManager.Instance.runtimePlayerData.blockLevel = 4;
            
            PlayerStatsPanel statsPanel = FindFirstObjectByType<PlayerStatsPanel>();
            if (statsPanel != null)
            {
                statsPanel.UpdateUI();
            }
        }
    }

    // Gojo buff: The Honored One (max everything - all stats and abilities)
    void ActivateGojo()
    {
        if (GameManager.Instance != null && GameManager.Instance.runtimePlayerData != null)
        {
            GameManager.Instance.runtimePlayerData.money += 999999;
            GameManager.Instance.runtimePlayerData.hp += 99999;
            GameManager.Instance.runtimePlayerData.damage += 999;
            GameManager.Instance.runtimePlayerData.moveSpeed += 99;
            GameManager.Instance.runtimePlayerData.attackSpeed += 99;
            GameManager.Instance.runtimePlayerData.bulletSpeed += 99;
            GameManager.Instance.runtimePlayerData.knockback += 99;
            GameManager.Instance.runtimePlayerData.piercingLevel = 4;
            GameManager.Instance.runtimePlayerData.dashLevel = 4;
            GameManager.Instance.runtimePlayerData.blockLevel = 4;
            GameManager.Instance.runtimePlayerData.freezeLevel = 4;
            GameManager.Instance.runtimePlayerData.numberOfSaves = 100;
            
            PlayerStatsPanel statsPanel = FindFirstObjectByType<PlayerStatsPanel>();
            if (statsPanel != null)
            {
                statsPanel.UpdateUI();
            }
        }
    }

    // Yuta/Megumi debuff: Bums and frauds (resets everything to minimum)
    void ActivateBums()
    {
        if (GameManager.Instance != null && GameManager.Instance.runtimePlayerData != null)
        {
            GameManager.Instance.runtimePlayerData.money = 0;
            GameManager.Instance.runtimePlayerData.hp = 1;
            GameManager.Instance.runtimePlayerData.damage = 1;
            GameManager.Instance.runtimePlayerData.moveSpeed = 1;
            GameManager.Instance.runtimePlayerData.attackSpeed = 1;
            GameManager.Instance.runtimePlayerData.bulletSpeed = 1;
            GameManager.Instance.runtimePlayerData.knockback = 1;
            GameManager.Instance.runtimePlayerData.piercingLevel = 0;
            GameManager.Instance.runtimePlayerData.dashLevel = 0;
            GameManager.Instance.runtimePlayerData.blockLevel = 0;
            GameManager.Instance.runtimePlayerData.freezeLevel = 0;
            GameManager.Instance.runtimePlayerData.numberOfSaves = 0;
            
            PlayerStatsPanel statsPanel = FindFirstObjectByType<PlayerStatsPanel>();
            if (statsPanel != null)
            {
                statsPanel.UpdateUI();
            }
        }
    }
}
