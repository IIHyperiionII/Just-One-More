using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameModeManager : MonoBehaviour
{
    public GameObject gameLoopParent;
    public GameObject casino;
    public GameObject escMenu;
    private bool inMiniGame = false;
    private bool escMenuActive = false;
    public Button casinoButton;
    private bool gameWonMenuActive = false;
    public GameObject gameWonMenu;
    private bool deadMenuActive = false;
    public GameObject deadMenu;
    public static bool timeIsPaused;
    public static bool isInSettingsMenu = false;
    public Button continueButton;
    public GameObject cameraDisortionEffect;


    void Start()
    {
        Application.targetFrameRate = 100;
        ExitMiniGame();
        casinoButton.onClick.AddListener(EnterMiniGame);
        continueButton.onClick.AddListener(CloseEscMenu);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !inMiniGame && !escMenuActive && !gameWonMenuActive && !deadMenuActive && !isInSettingsMenu)
        {
            OpenEscMenu();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && escMenuActive && !inMiniGame && !gameWonMenuActive && !deadMenuActive && !isInSettingsMenu)
        {
            CloseEscMenu();
        }
        if (escMenuActive || inMiniGame || gameWonMenuActive || deadMenuActive)
        {
            casinoButton.interactable = false;
        } else
        {
            casinoButton.interactable = true;
        }
        if (GameManager.Instance != null && GameManager.Instance.gameWon && !inMiniGame && !escMenuActive && !deadMenuActive)
        {
            gameWonMenuActive = true;
            GameWonMenu();
        }
        if (GameManager.Instance != null && GameManager.Instance.runtimePlayerData.isDead && !inMiniGame && !escMenuActive)
        {
            deadMenuActive = true;
            DeadMenu();
        }
    }
    public void OpenEscMenu()
    {
        escMenuActive = true;
        escMenu.SetActive(true);
        timeIsPaused = true;
    }

    public void CloseEscMenu()
    {
        escMenuActive = false;
        escMenu.SetActive(false);
        timeIsPaused = false;
    }

    public void EnterMiniGame()
    {
        inMiniGame = true;
        GameManager.Instance.runtimePlayerData.money -= 100;
        gameLoopParent.SetActive(false);
        cameraDisortionEffect.SetActive(false);
        casino.SetActive(true);
        timeIsPaused = true;
        if (SoundController.Instance != null)
        {
            SoundController.Instance.PlayCasinoMusic();
        }
    }

    public void ExitMiniGame()
    {
        inMiniGame = false;
        gameLoopParent.SetActive(true);
        casino.SetActive(false);
        cameraDisortionEffect.SetActive(true);
        timeIsPaused = false;
        if (SoundController.Instance != null)
        {
            SoundController.Instance.StopCasinoMusic();
        }
    }

    void GameWonMenu()
    {
        gameWonMenu.SetActive(true);
        timeIsPaused = true;
    }

    void DeadMenu()
    {
        deadMenu.SetActive(true);
        timeIsPaused = true;
    }

}

