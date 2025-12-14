using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class EscMenuController : MonoBehaviour
{
    public Button backToMainMenuButton;
    public Button saveGameButton;
    public Button quitGameButton;
    public Button settingsButton;
    public Button resetButton; // For testing purposes
    public TextMeshProUGUI saveGameText;
    public GameObject settingsMenu;

    void Start()
    {
        backToMainMenuButton.onClick.AddListener(BackToMainMenu);
        saveGameButton.onClick.AddListener(SaveGame);
        quitGameButton.onClick.AddListener(QuitGame);
        settingsButton.onClick.AddListener(OpenSettings);
        resetButton.onClick.AddListener(ResetButton); // For testing purposes

    }
    void Update()
    {
        if (saveGameText != null)
        {
            if (GameManager.Instance.runtimePlayerData.numberOfSaves <= 0)
            {
                saveGameButton.interactable = false;
            }
            else
            {
                saveGameButton.interactable = true;
            }
            saveGameText.text = $"Save game\n<size=50%>(Saves left: {GameManager.Instance.runtimePlayerData.numberOfSaves})</size>";
        }
    }

    void BackToMainMenu()
    {
        ModeController.Instance.ResetSettingsToDefault();
        SceneManager.LoadScene("MainMenuScene");
    }

    void SaveGame()
    {
        GameManager.Instance.runtimePlayerData.numberOfSaves -= 1;
        SaveSystem.Instance.SaveGame();
    }

    void QuitGame()
    {
        Application.Quit();
    }

    void OpenSettings()
    {
        GameModeManager.isInSettingsMenu = true;
        settingsMenu.SetActive(true);
        this.transform.parent.gameObject.SetActive(false);
    }
    // For testing purposes
    void ResetButton()
    {
        SaveSystem.Instance.ResetGameData();
    }
}
