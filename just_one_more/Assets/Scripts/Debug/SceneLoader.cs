using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Zavolej tuhle funkci místo přímého SceneManager.LoadScene()
    public void LoadGameplayScene()
    {
        StartCoroutine(LoadSceneAsync("tomScene")); // Změň "Gameplay" na jméno tvé scény
    }
    
    IEnumerator LoadSceneAsync(string sceneName)
    {
        Debug.Log($"Starting async load of scene: {sceneName}");
        
        // Vyčisti memory před načtením
        yield return Resources.UnloadUnusedAssets();
        System.GC.Collect();
        
        Debug.Log("Unused assets unloaded, starting scene load...");
        
        // Načti scénu asynchronně (postupně, ne najednou)
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false; // Nechceme aktivovat hned
        
        // Počkej než se načte většina (90%)
        while (asyncLoad.progress < 0.9f)
        {
            Debug.Log($"Loading progress: {asyncLoad.progress * 100}%");
            yield return null; // Počkej jeden frame
        }
        
        Debug.Log("Scene loaded 90%, waiting before activation...");
        
        // Extra pauza pro GPU breathing room
        yield return new WaitForSeconds(0.5f);
        
        // Teď aktivuj scénu
        asyncLoad.allowSceneActivation = true;
        
        // Počkej až se scéna skutečně aktivuje
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        Debug.Log("Scene fully loaded and activated!");
        
        // Ještě jednou vyčisti po načtení
        yield return Resources.UnloadUnusedAssets();
        
        Debug.Log("Post-load cleanup done!");
    }
}
