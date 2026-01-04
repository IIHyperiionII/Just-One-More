using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WonMenuController : MonoBehaviour
{
    public Button mainMenuButton;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI bestTimeText;
    private BestTimeSaveData.BestTimeData bestTimeData;
    public bool isReady = false;
    public bool isBestTime = false;
    public ModeAndWeaponSelection currentSelection;
    private string modeString;
    private string weaponString;
    void Awake()
    {
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        if (ModeController.Instance != null)
        {
            currentSelection = ModeController.Instance.currentSelection;
        }
    }
    void Start()
    {
        if (currentSelection == null)
        {
            currentSelection = ModeController.Instance.currentSelection;
        }
        if (SaveSystem.Instance != null)
        {
            bestTimeData = SaveSystem.Instance.currentBestTimeData;
        }
        if (bestTimeData != null)
        {
            // Check if current time is a new best time
            if (GameManager.Instance != null && GameManager.Instance.time < bestTimeData.bestTime || bestTimeData.bestTime == 0f)
            {
                isBestTime = true;
                bestTimeData.bestTime = GameManager.Instance.time;
                bestTimeData.selectedMode = currentSelection.selectedMode;
                bestTimeData.selectedWeapon = currentSelection.selectedWeapon;
                if (SaveSystem.Instance != null)
                {
                    SaveSystem.Instance.currentBestTimeData = bestTimeData;
                    SaveSystem.Instance.SaveBestTime();
                }
            }
        }
        isReady = true;
    }

    void Update()
    {
        // Update UI only when ready
        if (GameManager.Instance != null && isReady)
        {
            // Display final time
            float timeTaken = GameManager.Instance.time;
            timeText.text = $"Final time: {timeTaken:F2} seconds.";
            GetString();
            if (isBestTime)
            {
                bestTimeText.text = $"New Best Time: {timeTaken:F2} seconds!\n Mode: {modeString}\n Weapon: {weaponString}";

            }
            else
            {
                bestTimeText.text = $"Best Time: {bestTimeData.bestTime:F2} seconds.\n Mode: {modeString}\n Weapon: {weaponString}";
            
            }
        }  
    }
    void ReturnToMainMenu()
    {
        ModeController.Instance.ResetSettingsToDefault();
        // Stop game music and play main menu music
        if (SoundController.Instance != null)
        {
            SoundController.Instance.StopGameMusic();
            SoundController.Instance.PlayMainMenuMusic();
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
    }
    void GetString()
    {
        modeString = currentSelection.selectedMode.ToString();
        weaponString = currentSelection.selectedWeapon.ToString();
    }
}
