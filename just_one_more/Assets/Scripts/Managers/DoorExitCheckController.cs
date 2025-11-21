using System.Data;
using UnityEngine;

public class DoorExitCheckController : MonoBehaviour
{
    private GameObject gameManager;
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BoundsCheckPlayer") && GameManager.Instance.mapCompleted == false)
        {
            gameManager.GetComponent<GameManager>().background[GameManager.Instance.map + 3].SetActive(false);
            gameManager.GetComponent<GameManager>().background[GameManager.Instance.map].SetActive(true);
        }
    }
}
