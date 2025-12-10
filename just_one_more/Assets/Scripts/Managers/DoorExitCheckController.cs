using System.Collections;
using UnityEngine;

public class DoorExitCheckController : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BoundsCheckPlayer") && GameManager.Instance.mapCompleted == false)
        {
            GameManager.Instance.backgroundOpen[GameManager.Instance.map].SetActive(false);
            GameManager.Instance.background[GameManager.Instance.map].SetActive(true);
            GameManager.Instance.isTeleporting = false;
            GameManager.Instance.isOpen = false;
            SaveSystem.Instance.SaveGame();
        }
    }
}
