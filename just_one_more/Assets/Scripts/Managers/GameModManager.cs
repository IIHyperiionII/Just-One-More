using UnityEngine;
using UnityEngine.SceneManagement;

public class GameModeManager : MonoBehaviour
{
    public GameObject gameLoopParent;
    public GameObject miniGameParent;
    public GameObject Casino;
    public GameObject escMenu;
    private bool inMiniGame = false;
    private bool escMenuActive = false;
    public static bool playerInCasino;

    void Start()
    {
        Application.targetFrameRate = 100;
        if (Casino == null)
        {
            Casino = GameObject.FindGameObjectWithTag("Casino");
        }
        ExitMiniGame();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !inMiniGame && !escMenuActive)
        {
            escMenuActive = true;
            Time.timeScale = 0f;
            escMenu.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && escMenuActive)
        {
            escMenuActive = false;
            escMenu.SetActive(false);
            Time.timeScale = 1f;
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

}

