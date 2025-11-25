using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameModeManager : MonoBehaviour
{
    public GameObject gameLoopParent;
    public GameObject miniGameParent;
    public GameObject Casino;
    public GameObject escMenu;
    private bool inMiniGame = false;
    private bool escMenuActive = false;
    public Button casinoButton;
    private bool gameWonMenuActive = false;
    public GameObject gameWonMenu;
    private bool deadMenuActive = false;
    public GameObject deadMenu;
    public static bool playerInCasino;


    void Start()
    {
        Application.targetFrameRate = 100;
        if (Casino == null)
        {
            Casino = GameObject.FindGameObjectWithTag("Casino");
        }
        ExitMiniGame();
        casinoButton.onClick.AddListener(EnterMiniGame);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !inMiniGame && !escMenuActive)
        {
            escMenuActive = true;
            Time.timeScale = 0f;
            escMenu.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && escMenuActive && !inMiniGame && !gameWonMenuActive && !deadMenuActive)
        {
            escMenuActive = false;
            escMenu.SetActive(false);
            Time.timeScale = 1f;
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

    public void EnterMiniGame()
    {
        inMiniGame = true;
        gameLoopParent.SetActive(false);
        miniGameParent.SetActive(true);
        playerInCasino = true;
    }

    public void ExitMiniGame()
    {
        inMiniGame = false;
        gameLoopParent.SetActive(true);
        miniGameParent.SetActive(false);
        playerInCasino = false;
    }

    void GameWonMenu()
    {
        Time.timeScale = 0f;
        gameWonMenu.SetActive(true);
    }

    void DeadMenu()
    {
        Time.timeScale = 0f;
        deadMenu.SetActive(true);
    }

}

