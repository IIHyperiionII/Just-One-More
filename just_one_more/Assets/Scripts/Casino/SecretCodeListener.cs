using UnityEngine;
using Unity;
public class SecretCodeListener : MonoBehaviour
{
    private string currentInput = "";
    private int maxBufferLength = 8;
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
        if (gameModeManager == null || !gameModeManager.inMiniGame) return;
        
        if (Input.anyKeyDown)
        {
            foreach (char c in Input.inputString)
            {
                currentInput += c.ToString().ToLower();
                
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
        if (currentInput.EndsWith("mahoraga"))
        {
            ActivateMahoraga();
            currentInput = "";
        }
        else if (currentInput.EndsWith("gojo"))
        {
            ActivateGojo();
            currentInput = "";
        }
        else if (currentInput.EndsWith("yuta") || currentInput.EndsWith("megumi"))
        {
            ActivateBums();
            currentInput = "";
        }
    }

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
