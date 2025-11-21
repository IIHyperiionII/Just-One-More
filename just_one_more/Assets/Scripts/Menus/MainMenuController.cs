using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public Button startGameButton;
    public Button loadGameButton;
    public Button settingsButton;
    public Button quitGameButton;

    void Awake()
    {
        startGameButton.onClick.AddListener(StartGame);
        loadGameButton.onClick.AddListener(LoadGame);
        settingsButton.onClick.AddListener(OpenSettings);
        quitGameButton.onClick.AddListener(QuitGame);
    }
    void Start()
    {
        string savePath = Application.persistentDataPath + "/saveData.json";
        if ( System.IO.File.Exists(savePath) )
        {
            loadGameButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
        }
        else
        {
            loadGameButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
        }
        SaveSystem.Instance.LoadBestTime();
    }
    public void StartGame()
    {
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
        // Implement open settings functionality
    }
}
