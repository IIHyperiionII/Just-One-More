using UnityEngine;
using System.IO;
using UnityEngine.UI;
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

    string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }
    public void SaveGame()
    {
        Debug.Log("Saving Game...");
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        modeController = GameObject.FindGameObjectWithTag("ModeController").GetComponent<ModeController>();
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
        modeController.GetSaveData();
    }

    public void LoadGame()
    {
        Debug.Log("Loading Game...");
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        modeController = GameObject.FindGameObjectWithTag("ModeController").GetComponent<ModeController>();
    
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
        modeController.ApplySaveData();
        Time.timeScale = 1f;
    }

    IEnumerator WaitForSync()
    {
        yield return new WaitUntil(() => gameManager.isGameReadyToLoad && playerController.isReadyToLoad);
    }

    public void LoadBestTime()
    {
        string savePath = Path.Combine(Application.persistentDataPath, "BestTime.json");
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
        string savePath = Path.Combine(Application.persistentDataPath, "BestTime.json");
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Best Time file deleted");
        }
        string json = JsonUtility.ToJson(currentBestTimeData, true);
        File.WriteAllText(savePath, json);
    }
}
