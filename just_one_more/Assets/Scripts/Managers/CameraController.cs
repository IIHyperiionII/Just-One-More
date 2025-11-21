using UnityEngine;
using System.Collections;
using System.IO;
using NUnit.Framework;

public class CameraController : MonoBehaviour
{
    public Transform player;
    private static Vector3 originalPosition;
    public static bool isTeleporting = false;
    void Update()
    {
        if (GameModeManager.playerInCasino) return;
        if (player == null)
        {
            Debug.LogError("Player transform is not assigned in CameraController.");
        }
        else
        {
            if (isTeleporting) return;
            // Follow the player while maintaining the camera's z position
            Vector3 newPosition = player.position;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        }
    }

    public static void ShakeCamera(float duration = 0.2f, float magnitude = 0.2f)
    {
        GameObject cameraObject = Camera.main.gameObject;
        cameraObject.GetComponent<CameraController>().StartCoroutine(Shake(duration, magnitude, cameraObject.transform));
    }
    private static IEnumerator Shake(float duration, float magnitude, Transform cameraTransform)
    {
        Vector3 originalPos = cameraTransform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-0.5f, 0.5f) * magnitude;
            float y = Random.Range(-0.5f, 0.5f) * magnitude;

            cameraTransform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        cameraTransform.localPosition = originalPos;
    }

    public static IEnumerator TeleportMoveUp()
    {
        float time = 0f;
        Transform cameraPosition = Camera.main.transform;
        originalPosition = cameraPosition.position;

        while (time < 1f)
        {
            time += Time.unscaledDeltaTime / 3f; // Duration of 1 second
            cameraPosition.position = Vector3.Lerp(originalPosition, new Vector3(originalPosition.x, 40f, originalPosition.z), time);
            yield return null;
        }

    }

    public static IEnumerator TeleportMoveDown()
    {
        float time = 0f;
        Transform cameraPosition = Camera.main.transform;
        Vector3 currentPosition = cameraPosition.position;

        while (time < 1f)
        {
            time += Time.unscaledDeltaTime / 3f; // Duration of 1 second
            cameraPosition.position = Vector3.Lerp(currentPosition, new Vector3(currentPosition.x, originalPosition.y, currentPosition.z), time);
            yield return null;
        }
    }
}
