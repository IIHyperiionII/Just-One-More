using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Collections;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance;
    private GameManager gameManager;
    private PlayerController playerController;
    public bool toLoad = false;
    private string fileName = "saveData.json";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public SaveData currentSaveData = new SaveData();
    

    string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }
    public void SaveGame()
    {
        Debug.Log("Saving Game...");
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        UpdateSaveData();

        string savePath = GetFilePath();
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save file deleted");
        }
        // Convert the SaveData object to JSON
        string json = JsonUtility.ToJson(currentSaveData, true);

        // Write JSON to file
        File.WriteAllText(savePath, json);

        Debug.Log("Game saved to: " + savePath);
    }

    private void UpdateSaveData()
    {
        gameManager.GetSaveData();
        playerController.GetSaveData();
    }

    public void LoadGame()
    {
        Debug.Log("Loading Game...");
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    
        Time.timeScale = 0f;
        string savePath = GetFilePath();
        
        StartCoroutine(WaitForSync());

        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            currentSaveData = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            Debug.LogWarning("Save file not found!");
            currentSaveData = new SaveData();
        }
        playerController.ApplySaveData();
        gameManager.ApplySaveData();
        Time.timeScale = 1f;
    }

    IEnumerator WaitForSync()
    {
        yield return new WaitUntil(() => gameManager.isGameReadyToLoad && playerController.isReadyToLoad);
    }
}
