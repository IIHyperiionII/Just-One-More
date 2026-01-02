using UnityEngine;
using Unity;
public class SecretCodeListener : MonoBehaviour
{
    // Add: Gojo (all max), megumi/yuta (all min)
    private string secretCode = "mahoraga";
    private string currentInput = "";
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
                
                if (currentInput.Length > secretCode.Length)
                {
                    currentInput = currentInput.Substring(1);
                }
                
                if (currentInput == secretCode)
                {
                    OnSecretCodeEntered();
                    currentInput = "";
                }
            }
        }
    }
    
    void OnSecretCodeEntered()
    {
        if (GameManager.Instance != null && GameManager.Instance.runtimePlayerData != null)
        {
            GameManager.Instance.runtimePlayerData.money += 999999;
            GameManager.Instance.runtimePlayerData.hp += 99999;
            GameManager.Instance.runtimePlayerData.damage += 999;
            GameManager.Instance.runtimePlayerData.moveSpeed += 99;
            GameManager.Instance.runtimePlayerData.dashLevel += 99;
            GameManager.Instance.runtimePlayerData.numberOfSaves += 99;
            GameManager.Instance.runtimePlayerData.blockLevel += 99;
            
            PlayerStatsPanel statsPanel = FindFirstObjectByType<PlayerStatsPanel>();
            if (statsPanel != null)
            {
                statsPanel.UpdateUI();
            }
        }
    }
}
