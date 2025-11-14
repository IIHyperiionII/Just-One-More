using UnityEngine;
using UnityEngine.SceneManagement;

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
    }
    
    void Update()
    {
        if (miniPhysicsScene.IsValid())
        {
            miniPhysicsScene.Simulate(Time.unscaledDeltaTime);
        }
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
        Time.timeScale = 0f;
    }

    public void ExitMiniGame()
    {
        inMiniGame = false;
        gameLoopParent.SetActive(true);
        miniGameParent.SetActive(false);
        Time.timeScale = 1f;
    }

}

