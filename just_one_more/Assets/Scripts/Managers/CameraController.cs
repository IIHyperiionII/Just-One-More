using UnityEngine;
using System.Collections;
using System.IO;
using NUnit.Framework;
using System.Runtime.Serialization.Formatters;

public class CameraController : MonoBehaviour
{
    public Transform player;
    private static Vector3 originalPosition;
    public static bool isTeleporting = false;
    private static Vector3 newPosition;
    private static bool isShaking = false;
    void Update()
    {
        if (player == null)
        {
            Debug.LogError("Player transform is not assigned in CameraController.");
        }
        else
        {
            if (isTeleporting) return;
            // Follow the player while maintaining the camera's z position
            newPosition = player.position;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        }
    }

    public static void ShakeCamera(float duration = 0.2f, float magnitude = 0.2f)
    {
        if (isShaking) return;
        GameObject cameraObject = Camera.main.gameObject;
        cameraObject.GetComponent<CameraController>().StartCoroutine(Shake(duration, magnitude, cameraObject.transform));
    }
    private static IEnumerator Shake(float duration, float magnitude, Transform cameraTransform)
    {
        float elapsed = 0.0f;
        isShaking = true;

        while (elapsed < duration)
        {
            if (Time.timeScale == 0) yield break;
            float x = Random.Range(-0.5f, 0.5f) * magnitude;
            float y = Random.Range(-0.5f, 0.5f) * magnitude;

            cameraTransform.position = new Vector3(newPosition.x + x, newPosition.y + y, newPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }
        isShaking = false;
        cameraTransform.position = newPosition;
    }
    public static IEnumerator PulseCamera(float intensity, float duration)
    {
        GameObject cameraObject = Camera.main.gameObject;
        cameraObject.GetComponent<CameraController>().StartCoroutine(HeartbeatPulse(intensity * 0.6f, duration, cameraObject.transform));
        yield return new WaitForSeconds(duration);
        cameraObject.GetComponent<CameraController>().StartCoroutine(Shake(duration * 0.5f, intensity * 0.5f, cameraObject.transform));
        yield return new WaitForSeconds(duration * 0.5f +0.3f);
        cameraObject.GetComponent<CameraController>().StartCoroutine(HeartbeatPulse(intensity, duration, cameraObject.transform));
        yield return new WaitForSeconds(duration);
        cameraObject.GetComponent<CameraController>().StartCoroutine(Shake(duration * 0.5f, intensity * 1f, cameraObject.transform));
    }
    public static IEnumerator HeartbeatPulse(float intensity, float duration, Transform cameraTransform)
    {
        float t = 0f;
        float pulseDuration = duration;
        float originalSize = cameraTransform.GetComponent<Camera>().orthographicSize;
        float targetSize = originalSize + intensity;

        // scale up
        while (t < pulseDuration)
        {
            t += Time.deltaTime;
            float normalized = t / pulseDuration;
            cameraTransform.GetComponent<Camera>().orthographicSize = Mathf.Lerp(originalSize, targetSize, normalized);
            yield return null;
        }

        t = 0f;
        yield return new WaitForSeconds(duration * 0.5f);


        cameraTransform.GetComponent<Camera>().orthographicSize = originalSize;
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
