using UnityEngine;

public class DoorsController : MonoBehaviour
{
    // This script handles the player's interaction with doors in the game.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BoundsCheckPlayer") && GameManager.Instance.mapCompleted == true)
        {
            GameManager.Instance.doorsEntered = true;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("BoundsCheckPlayer"))
        {
            GameManager.Instance.doorsEntered = false;
        }
    }

}
