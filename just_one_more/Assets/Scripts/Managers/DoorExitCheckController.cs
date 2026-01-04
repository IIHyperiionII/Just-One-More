using System.Collections;
using UnityEngine;

public class DoorExitCheckController : MonoBehaviour
{

    // This method is called when another collider enters the trigger collider attached to the object where this script is applied.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DoorExitCheck") && GameManager.Instance.mapCompleted == false)
        {
            GameManager.Instance.backgroundOpen[GameManager.Instance.map].SetActive(false);
            GameManager.Instance.background[GameManager.Instance.map].SetActive(true);
            GameManager.Instance.isTeleporting = false;
            GameManager.Instance.isOpen = false;
            SaveSystem.Instance.SaveGame();
        }
    }
}
