using UnityEngine;
using System.IO;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance;
    private GameManager gameManager;
    private PlayerController playerController;
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

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    public SaveData currentSaveData = new SaveData();
    

    string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }
    public void SaveGame()
    {
        UpdateSaveData();

        string savePath = GetFilePath();

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
        Time.timeScale = 0f;
        string savePath = GetFilePath();

        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            currentSaveData = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("Game loaded from: " + savePath);
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
}
