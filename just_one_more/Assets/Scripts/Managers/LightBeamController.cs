using UnityEngine;

public class LightBeamController : MonoBehaviour
{
    void Update()
    {
        if (GameModeManager.timeIsPaused) return;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Get mouse position in world space
        mousePos.z = this.transform.position.z;

        Vector2 direction = mousePos - this.transform.position; // Calculate direction from beam to mouse position
        if (direction.sqrMagnitude < 0.0001f) return; // Avoid zero-length direction

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Calculate angle in degrees
        targetAngle += 90f; // Adjusting angle to point the beam correctly
        Quaternion targetRotationMouse = Quaternion.Euler(0f, 0f, targetAngle); // Create rotation from angle
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, targetRotationMouse, Time.deltaTime * 20f); // Smoothly rotate towards target rotation
    }
}
