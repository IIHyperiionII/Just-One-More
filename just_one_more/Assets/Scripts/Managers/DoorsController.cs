using UnityEngine;

public class DoorsController : MonoBehaviour
{
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
