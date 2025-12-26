using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeadMenuController : MonoBehaviour
{
    public Button mainMenuButton;
    public TextMeshProUGUI timeText;
    public bool isBestTime = false;
    void Awake()
    {
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    void Update()
    {
        if (GameManager.Instance != null)
        {
            float timeTaken = GameManager.Instance.time;
            timeText.text = $"You survived for {timeTaken:F2} seconds.";
        }  
    }
    void ReturnToMainMenu()
    {
        ModeController.Instance.ResetSettingsToDefault();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
    }

}
