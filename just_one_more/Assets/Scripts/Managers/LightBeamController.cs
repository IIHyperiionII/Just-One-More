using UnityEngine;

public class LightBeamController : MonoBehaviour
{
    void Update()
    {
        if (GameModeManager.timeIsPaused) return;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = this.transform.position.z;

        Vector2 direction = mousePos - this.transform.position;
        if (direction.sqrMagnitude < 0.0001f) return;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        targetAngle += 90f; // Adjusting angle to point the beam correctly
        Quaternion targetRotationMouse = Quaternion.Euler(0f, 0f, targetAngle);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, targetRotationMouse, Time.deltaTime * 20f);
    }
}
