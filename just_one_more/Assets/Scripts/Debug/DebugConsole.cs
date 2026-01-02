using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class DebugConsole : MonoBehaviour
{
    private List<string> logs = new List<string>();
    private bool showConsole = false;
    private Vector2 scrollPosition;
    private string filterText = "";
    
    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }
    
    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }
    
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string prefix = "";
        switch(type)
        {
            case LogType.Error:
            case LogType.Exception:
                prefix = "[ERROR] ";
                break;
            case LogType.Warning:
                prefix = "[WARNING] ";
                break;
        }
        
        logs.Add(prefix + logString);
        if(logs.Count > 500) logs.RemoveAt(0); // Keep last 500
    }
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            showConsole = !showConsole;
        }
        
        // ✅ F2 - Dump aktuálního stavu
        if(Input.GetKeyDown(KeyCode.F2))
        {
            DumpCurrentState();
        }
        
        // ✅ F3 - Kontrola ModeController
        if(Input.GetKeyDown(KeyCode.F3))
        {
            CheckModeController();
        }
        
        // ✅ F4 - Kontrola GameManager
        if(Input.GetKeyDown(KeyCode.F4))
        {
            CheckGameManager();
        }
        
        // ✅ F5 - Kontrola PlayerStatsPanel
        if(Input.GetKeyDown(KeyCode.F5))
        {
            CheckPlayerStatsPanel();
        }
    }
    
    void DumpCurrentState()
    {
        Debug.Log("========== COMPLETE STATE DUMP ==========");
        CheckModeController();
        CheckGameManager();
        CheckPlayerStatsPanel();
        Debug.Log("=========================================");
    }
    
    void CheckModeController()
    {
        Debug.Log("--- ModeController Check ---");
        
        if(ModeController.Instance == null)
        {
            Debug.LogError("ModeController.Instance is NULL!");
            return;
        }
        
        Debug.Log("ModeController.Instance EXISTS");
        
        if(ModeController.Instance.currentSelection == null)
        {
            Debug.LogError("ModeController.Instance.currentSelection is NULL!");
            return;
        }
        
        var selection = ModeController.Instance.currentSelection;
        Debug.Log($"currentSelection.selectedWeapon = {selection.selectedWeapon}");
        Debug.Log($"currentSelection.selectedMode = {selection.selectedMode}");
        Debug.Log($"Is Melee? {selection.selectedWeapon == WeaponType.Melee}");
    }
    
    void CheckGameManager()
    {
        Debug.Log("--- GameManager Check ---");
        
        if(GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is NULL!");
            return;
        }
        
        Debug.Log("GameManager.Instance EXISTS");
        
        if(GameManager.Instance.runtimePlayerData == null)
        {
            Debug.LogError("GameManager.Instance.runtimePlayerData is NULL!");
            return;
        }
        
        var playerData = GameManager.Instance.runtimePlayerData;
        Debug.Log($"runtimePlayerData.isMelee = {playerData.isMelee}");
        Debug.Log($"runtimePlayerData.hp = {playerData.hp}");
        Debug.Log($"runtimePlayerData.damage = {playerData.damage}");
        Debug.Log($"runtimePlayerData.knockback = {playerData.knockback}");
        Debug.Log($"runtimePlayerData.bulletSpeed = {playerData.bulletSpeed}");
        
        if(GameManager.Instance.basePlayerData != null)
        {
            Debug.Log($"basePlayerData.isMelee = {GameManager.Instance.basePlayerData.isMelee}");
        }
    }
    
    void CheckPlayerStatsPanel()
    {
        Debug.Log("--- PlayerStatsPanel Check ---");
        
        var statsPanel = FindFirstObjectByType<PlayerStatsPanel>();
        
        if(statsPanel == null)
        {
            Debug.LogWarning("PlayerStatsPanel not found in scene!");
            return;
        }
        
        Debug.Log("PlayerStatsPanel EXISTS");
        
        var playerData = statsPanel.GetPlayerData();
        if(playerData == null)
        {
            Debug.LogError("PlayerStatsPanel.playerData is NULL!");
            return;
        }
        
        Debug.Log($"PlayerStatsPanel.playerData.isMelee = {playerData.isMelee}");
        Debug.Log($"PlayerStatsPanel.GetAttackModifierName() = {statsPanel.GetAttackModifierName()}");
        Debug.Log($"PlayerStatsPanel.GetAttackModifierStatType() = {statsPanel.GetAttackModifierStatType()}");
        Debug.Log($"PlayerStatsPanel.GetAttackModifier() = {statsPanel.GetAttackModifier()}");
    }
    
    void OnGUI()
    {
        if(!showConsole) return;
        
        // Černé poloprůhledné pozadí
        GUI.color = new Color(0, 0, 0, 0.9f);
        GUI.Box(new Rect(10, 10, Screen.width - 20, Screen.height - 20), "");
        GUI.color = Color.white;
        
        GUILayout.BeginArea(new Rect(20, 20, Screen.width - 40, Screen.height - 40));
        
        GUILayout.Label("=== DEBUG CONSOLE ===");
        GUILayout.Label("F1: Toggle Console | F2: Full Dump | F3: ModeController | F4: GameManager | F5: PlayerStatsPanel");
        GUILayout.Label("Total logs: " + logs.Count);
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("Filter:", GUILayout.Width(50));
        filterText = GUILayout.TextField(filterText, GUILayout.Width(300));
        if(GUILayout.Button("Clear Filter", GUILayout.Width(100)))
        {
            filterText = "";
        }
        if(GUILayout.Button("Clear Logs", GUILayout.Width(100)))
        {
            logs.Clear();
        }
        GUILayout.EndHorizontal();
        
        // ✅ Quick info panel
        GUILayout.BeginVertical("box");
        GUILayout.Label("=== QUICK INFO ===");
        
        if(GameManager.Instance != null && GameManager.Instance.runtimePlayerData != null)
        {
            var pd = GameManager.Instance.runtimePlayerData;
            GUILayout.Label($"isMelee: {pd.isMelee} | HP: {pd.hp} | DMG: {pd.damage}");
            GUILayout.Label($"Knockback: {pd.knockback} | BulletSpeed: {pd.bulletSpeed}");
        }
        else
        {
            GUILayout.Label("GameManager or runtimePlayerData is NULL!");
        }
        
        if(ModeController.Instance != null && ModeController.Instance.currentSelection != null)
        {
            var sel = ModeController.Instance.currentSelection;
            GUILayout.Label($"Selected: {sel.selectedWeapon} | Mode: {sel.selectedMode}");
        }
        else
        {
            GUILayout.Label("ModeController or currentSelection is NULL!");
        }
        
        GUILayout.EndVertical();
        
        GUILayout.Space(10);
        
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        
        foreach(string log in logs)
        {
            if(string.IsNullOrEmpty(filterText) || log.ToLower().Contains(filterText.ToLower()))
            {
                // ✅ Barevné zvýraznění důležitých logů
                if(log.Contains("[ERROR]"))
                {
                    GUI.color = Color.red;
                }
                else if(log.Contains("[WARNING]"))
                {
                    GUI.color = Color.yellow;
                }
                else if(log.Contains("isMelee") || log.Contains("selectedWeapon") || log.Contains("Melee"))
                {
                    GUI.color = Color.cyan;
                }
                else
                {
                    GUI.color = Color.white;
                }
                
                GUILayout.Label(log);
            }
        }
        
        GUI.color = Color.white;
        GUILayout.EndScrollView();
        
        GUILayout.EndArea();
    }
}
