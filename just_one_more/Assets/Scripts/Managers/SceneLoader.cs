using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

// Handles async scene loading with memory cleanup to prevent crashes
// Persists across scenes via DontDestroyOnLoad
public class SceneLoader : MonoBehaviour
{

    void Awake()
    {
        // Persist this manager across all scene loads
        DontDestroyOnLoad(gameObject);
    }

    // Call this instead of direct SceneManager.LoadScene() to prevent memory issues
    public void LoadGameplayScene()
    {
        StartCoroutine(LoadSceneAsync("tomScene"));
    }
    
    IEnumerator LoadSceneAsync(string sceneName)
    {
        Debug.Log($"Starting async load of scene: {sceneName}");
        
        // Clean up memory before loading new scene
        yield return Resources.UnloadUnusedAssets();
        System.GC.Collect();
        
        Debug.Log("Unused assets unloaded, starting scene load...");
        
        // Load scene asynchronously (gradual, not all at once)
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false; // Don't activate immediately
        
        // Wait until most of scene is loaded (90%)
        while (asyncLoad.progress < 0.9f)
        {
            Debug.Log($"Loading progress: {asyncLoad.progress * 100}%");
            yield return null; // Wait one frame
        }
        
        Debug.Log("Scene loaded 90%, waiting before activation...");
        
        // Extra pause to give GPU breathing room
        yield return new WaitForSeconds(0.5f);
        
        // Now activate the scene
        asyncLoad.allowSceneActivation = true;
        
        // Wait until scene is actually activated
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        Debug.Log("Scene fully loaded and activated!");
        
        // Clean up again after loading
        yield return Resources.UnloadUnusedAssets();
        
        Debug.Log("Post-load cleanup done!");
    }
}
