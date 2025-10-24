using UnityEngine;
using UnityEngine.SceneManagement;

public class GameModeManager : MonoBehaviour
{
    public GameObject gameLoopParent;
    public GameObject miniGameParent;
    public PhysicsScene2D miniPhysicsScene;
    private Scene miniScene;
    public GameObject Casino;


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
            miniPhysicsScene.Simulate(Time.fixedDeltaTime);
        }
    }

    public void EnterMiniGame()
    {
        gameLoopParent.SetActive(false);
        miniGameParent.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ExitMiniGame()
    {
        gameLoopParent.SetActive(true);
        miniGameParent.SetActive(false);
        Time.timeScale = 1f;
    }
}

