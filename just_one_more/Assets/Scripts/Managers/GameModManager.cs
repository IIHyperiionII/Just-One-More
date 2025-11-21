using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameModeManager : MonoBehaviour
{
    public GameObject gameLoopParent;
    public GameObject miniGameParent;
    public PhysicsScene2D miniPhysicsScene;
    private Scene miniScene;
    public GameObject Casino;
    public GameObject escMenu;
    private bool inMiniGame = false;
    private bool escMenuActive = false;
    public Button casinoButton;
    private bool gameWonMenuActive = false;
    public GameObject gameWonMenu;


    void Start()
    {
        Application.targetFrameRate = 100;
        miniScene = SceneManager.CreateScene("MiniGamePhysicsScene", new CreateSceneParameters(LocalPhysicsMode.Physics2D));
        miniPhysicsScene = miniScene.GetPhysicsScene2D();
        if (Casino == null)
        {
            Casino = GameObject.FindGameObjectWithTag("Casino");
        }
        else
        {
            SceneManager.MoveGameObjectToScene(Casino, miniScene);
        }
        ExitMiniGame();
        casinoButton.onClick.AddListener(EnterMiniGame);
    }
    
    void Update()
    {
        if (miniPhysicsScene.IsValid())
        {
            miniPhysicsScene.Simulate(Time.unscaledDeltaTime);
        }
        if (Input.GetKeyDown(KeyCode.Escape) && !inMiniGame && !escMenuActive && !gameWonMenuActive )
        {
            escMenuActive = true;
            Time.timeScale = 0f;
            escMenu.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && escMenuActive && !inMiniGame && !gameWonMenuActive)
        {
            escMenuActive = false;
            escMenu.SetActive(false);
            Time.timeScale = 1f;
        }
        if (escMenuActive || inMiniGame || gameWonMenuActive)
        {
            casinoButton.interactable = false;
        } else
        {
            casinoButton.interactable = true;
        }
        if (GameManager.Instance != null && GameManager.Instance.gameWon && !inMiniGame && !escMenuActive)
        {
            gameWonMenuActive = true;
            GameWonMenu();
        }
    }

    public void EnterMiniGame()
    {
        inMiniGame = true;
        gameLoopParent.SetActive(false);
        miniGameParent.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ExitMiniGame()
    {
        inMiniGame = false;
        gameLoopParent.SetActive(true);
        miniGameParent.SetActive(false);
        Time.timeScale = 1f;
    }

    void GameWonMenu()
    {
        Time.timeScale = 0f;
        gameWonMenu.SetActive(true);
    }

}

