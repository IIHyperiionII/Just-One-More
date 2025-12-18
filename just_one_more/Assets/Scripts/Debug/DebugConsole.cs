using UnityEngine;
using System.Collections.Generic;

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
        if(logs.Count > 200) logs.RemoveAt(0); // Keep last 200
    }
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            showConsole = !showConsole;
        }
    }
    
    void OnGUI()
    {
        if(!showConsole) return;
        
        // Černé poloprůhledné pozadí
        GUI.color = new Color(0, 0, 0, 0.9f);
        GUI.Box(new Rect(10, 10, Screen.width - 20, Screen.height - 20), "");
        GUI.color = Color.white;
        
        GUILayout.BeginArea(new Rect(20, 20, Screen.width - 40, Screen.height - 40));
        
        GUILayout.Label("Debug Console (F1 to toggle) - Total logs: " + logs.Count);
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("Filter:", GUILayout.Width(50));
        filterText = GUILayout.TextField(filterText, GUILayout.Width(200));
        if(GUILayout.Button("Clear Filter", GUILayout.Width(100)))
        {
            filterText = "";
        }
        if(GUILayout.Button("Clear Logs", GUILayout.Width(100)))
        {
            logs.Clear();
        }
        GUILayout.EndHorizontal();
        
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        
        foreach(string log in logs)
        {
            if(string.IsNullOrEmpty(filterText) || log.ToLower().Contains(filterText.ToLower()))
            {
                GUILayout.Label(log);
            }
        }
        
        GUILayout.EndScrollView();
        
        GUILayout.EndArea();
    }
}
