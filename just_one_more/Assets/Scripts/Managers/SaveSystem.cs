using UnityEngine;
using System.IO;
using System.Collections;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance;
    private GameManager gameManager;
    private PlayerController playerController;
    private ModeController modeController;
    public bool toLoad = false;
    public bool isNewGame = false;
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

    // Data Structures to hold save data
    public SaveData currentSaveData = new SaveData();
    public BestTimeSaveData.BestTimeData currentBestTimeData = new BestTimeSaveData.BestTimeData();
    
    void Update()
    {
        if (GameManager.Instance != null && isNewGame)
        {
            GameManager.Instance.ResetGameManager();
            isNewGame = false;
        }
    }

    // Get the file path for saving/loading
    string GetFilePath(string fileName)
    {
        string folderPath = Application.persistentDataPath; // Default save location

        // Differentiate between Editor and Build saves
        if (!Application.isEditor)
        {
            folderPath = Path.Combine(folderPath, "BuildSaves");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
        }

        return Path.Combine(folderPath, fileName);
    }
    public void SaveGame()
    {
        Debug.Log("Saving Game...");
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        UpdateSaveData();

        string savePath = GetFilePath(fileName);
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
        GameManager.Instance.GetSaveData();
        playerController.GetSaveData();
        ModeController.Instance.GetSaveData();
    }

    public void LoadGame()
    {
        Debug.Log("Loading Game...");
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        modeController = GameObject.FindGameObjectWithTag("ModeController").GetComponent<ModeController>();
    
        Time.timeScale = 0f;
        string savePath = GetFilePath(fileName);
        
        StartCoroutine(WaitForSync());
        
        // Read JSON from file and convert back to SaveData object
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
        modeController.ApplySaveData();
        playerController.ApplySaveData();
        gameManager.ApplySaveData();
        Time.timeScale = 1f;
    }

    IEnumerator WaitForSync()
    {
        yield return new WaitUntil(() => gameManager.isGameReadyToLoad && playerController.isReadyToLoad);
    }

    public void LoadBestTime()
    {
        string savePath = GetFilePath("BestTime.json");
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            currentBestTimeData = JsonUtility.FromJson<BestTimeSaveData.BestTimeData>(json);
        }
        else
        {
            Debug.LogWarning("Best Time file not found!");
            currentBestTimeData = new BestTimeSaveData.BestTimeData();
        }
    }

    public void SaveBestTime()
    {
        string savePath = GetFilePath("BestTime.json");

        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Best Time file deleted");
        }
        string json = JsonUtility.ToJson(currentBestTimeData, true);
        File.WriteAllText(savePath, json);
    }

    public void ResetGameData()
    {
        string savePath = GetFilePath(fileName);
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save file deleted for reset");
        }
        currentSaveData = new SaveData();
    }
}
