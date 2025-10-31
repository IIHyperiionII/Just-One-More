using UnityEngine;

public class DoorsController : MonoBehaviour
{
    private GameObject gameManager;
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BoundsCheckPlayer"))
        {
            gameManager.GetComponent<GameManager>().doorsEntered = true;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("BoundsCheckPlayer"))
        {
            gameManager.GetComponent<GameManager>().doorsEntered = false;
        }
    }
}
