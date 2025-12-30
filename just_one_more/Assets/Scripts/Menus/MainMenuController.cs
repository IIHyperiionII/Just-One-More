using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    public Button startGameButton;
    public Button loadGameButton;
    public Button settingsButton;
    public Button quitGameButton;
    public GameObject settingsMenu;

    void Awake()
    {
        startGameButton.onClick.AddListener(StartGame);
        loadGameButton.onClick.AddListener(LoadGame);
        settingsButton.onClick.AddListener(OpenSettings);
        quitGameButton.onClick.AddListener(QuitGame);
    }
    void Start()
    {
        string savePath = GetSavePath();
        loadGameButton.GetComponent<UnityEngine.UI.Button>().interactable = File.Exists(savePath);
        SaveSystem.Instance.LoadBestTime();
    }

    void Update()
    {
        string savePath = GetSavePath();
        loadGameButton.GetComponent<UnityEngine.UI.Button>().interactable = File.Exists(savePath);
        if (loadGameButton.GetComponent<UnityEngine.UI.Button>().interactable)
        {
            loadGameButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            loadGameButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
    }

    string GetSavePath()
    {
        string fileName = "saveData.json";
        string folderPath = Application.persistentDataPath;

        if (!Application.isEditor)
        {
            folderPath = Path.Combine(folderPath, "BuildSaves");
        }

        return Path.Combine(folderPath, fileName);
    }

    public void StartGame()
    {
        SaveSystem.Instance.isNewGame = true;
        SceneManager.LoadScene("ModeAndWeaponSelectionMenu");
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadGame()
    {
        Debug.Log("Loading saved game...");
        SaveSystem.Instance.toLoad = true;
        SceneManager.LoadScene("tomScene");
    }
    public void OpenSettings()
    {
        settingsMenu.SetActive(true);
        SettingsController.isFromMainMenu = true;
    }
}
