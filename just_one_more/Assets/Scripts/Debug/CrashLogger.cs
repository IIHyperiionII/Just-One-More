using UnityEngine;
using UnityEngine.SceneManagement;

public class CrashLogger : MonoBehaviour
{
    void Awake()
    {
        // Tohle zajistí že CrashLogger přežije všechny scene loadingy
        DontDestroyOnLoad(gameObject);
        
        // Zachytávej všechny error messages
        Application.logMessageReceived += HandleLog;
        
        // Zaloguj systémové info při startu
        Debug.Log("========================================");
        Debug.Log("=== GAME STARTED ===");
        Debug.Log($"Time: {System.DateTime.Now}");
        Debug.Log($"Unity Version: {Application.unityVersion}");
        Debug.Log("========================================");
        Debug.Log("=== SYSTEM INFO ===");
        Debug.Log($"OS: {SystemInfo.operatingSystem}");
        Debug.Log($"Processor: {SystemInfo.processorType}");
        Debug.Log($"Processor Count: {SystemInfo.processorCount}");
        Debug.Log($"System Memory: {SystemInfo.systemMemorySize} MB");
        Debug.Log("========================================");
        Debug.Log("=== GRAPHICS INFO ===");
        Debug.Log($"Graphics Device: {SystemInfo.graphicsDeviceName}");
        Debug.Log($"Graphics Memory: {SystemInfo.graphicsMemorySize} MB");
        Debug.Log($"Graphics API: {SystemInfo.graphicsDeviceType}");
        Debug.Log($"Graphics Shader Level: {SystemInfo.graphicsShaderLevel}");
        Debug.Log($"Max Texture Size: {SystemInfo.maxTextureSize}");
        Debug.Log($"Supports Compute Shaders: {SystemInfo.supportsComputeShaders}");
        Debug.Log("========================================");
        Debug.Log($"Screen Resolution: {Screen.currentResolution.width}x{Screen.currentResolution.height}");
        Debug.Log($"Quality Level: {QualitySettings.names[QualitySettings.GetQualityLevel()]}");
        Debug.Log("========================================");
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Zaloguj všechny errory a exceptions extra viditelně
        if (type == LogType.Error || type == LogType.Exception)
        {
            Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            Debug.Log("=== ERROR DETECTED ===");
            Debug.Log($"Scene: {SceneManager.GetActiveScene().name}");
            Debug.Log($"Time: {Time.time}s");
            Debug.Log($"Error Type: {type}");
            Debug.Log($"Message: {logString}");
            Debug.Log($"Stack Trace: {stackTrace}");
            Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        }
    }
    
    void Update()
    {
        // Každých 5 sekund zaloguj memory usage
        if (Time.frameCount % 300 == 0) // 60 FPS * 5 sec = 300 frames
        {
            long memoryUsed = System.GC.GetTotalMemory(false);
            Debug.Log($"[{Time.time:F1}s] Memory in use: {memoryUsed / 1024 / 1024} MB | Scene: {SceneManager.GetActiveScene().name}");
        }
    }

    void OnApplicationQuit()
    {
        Debug.Log("========================================");
        Debug.Log("=== GAME CLOSING ===");
        Debug.Log($"Time: {System.DateTime.Now}");
        Debug.Log($"Total playtime: {Time.time}s");
        Debug.Log("========================================");
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }
}
