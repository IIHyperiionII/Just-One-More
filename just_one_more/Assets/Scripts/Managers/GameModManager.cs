using UnityEngine;
using UnityEngine.SceneManagement;

public class GameModeManager : MonoBehaviour
{
    public GameObject gameLoopParent;
    public GameObject miniGameParent;
    public PhysicsScene2D miniPhysicsScene;
    public GameObject Casino;
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

    public void EnterMiniGame()
    {
        gameLoopParent.SetActive(false);
        miniGameParent.SetActive(true);
        playerInCasino = true;
    }

    public void ExitMiniGame()
    {
        gameLoopParent.SetActive(true);
        miniGameParent.SetActive(false);
        playerInCasino = false;
    }
}

