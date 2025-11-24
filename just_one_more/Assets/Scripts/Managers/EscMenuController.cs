using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EscMenuController : MonoBehaviour
{
    public Button backToMainMenuButton;
    public Button saveGameButton;
    public Button quitGameButton;
    public Button settingsButton;

    void Start()
    {
        backToMainMenuButton.onClick.AddListener(BackToMainMenu);
        saveGameButton.onClick.AddListener(SaveGame);
        quitGameButton.onClick.AddListener(QuitGame);
        settingsButton.onClick.AddListener(OpenSettings);
    }

    void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    void SaveGame()
    {
        SaveSystem.Instance.SaveGame();
    }

    void QuitGame()
    {
        Application.Quit();
    }

    void OpenSettings()
    {
        // Implement open settings functionality
    }
}
