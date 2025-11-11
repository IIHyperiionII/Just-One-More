using UnityEngine;

public class HandFollowCursor : MonoBehaviour
{
    [Header("References")]
    public Transform handTransform;        
    public SpriteRenderer handRenderer;      
    public SpriteRenderer bodyRenderer;      

[Header("Rotation")]
    public float rotationSpeed = 15f;
    public float rotationOffset = 0f;

    [Header("Offsets")]
    public Vector3 offsetUp = new Vector3(-0.21f, 0.21f, 0f);
    public Vector3 offsetLeft = new Vector3(0.05f, 0.12f, 0f);
    public Vector3 offsetDown = new Vector3(0.21f, 0.21f, 0f);
    public Vector3 offsetRight = new Vector3(0f, 0.1f, 0f);

    [Header("Sorting")]
    public int frontOffsetOrder = 1;
    public int backOffsetOrder = -1;

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;

        if (handTransform == null)
            handTransform = transform.GetChild(0); 
        if (handRenderer == null && handTransform != null)
            handRenderer = handTransform.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (mainCam == null || handTransform == null) return;

        Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = handTransform.position.z;
        Vector2 direction = mousePos - handTransform.position;
        if (direction.sqrMagnitude < 0.0001f) return;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + rotationOffset;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
        handTransform.rotation = Quaternion.Lerp(handTransform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    public void ApplyLookDir(int lookDir)
    {
        if (handTransform == null) return;

     
        switch (lookDir)
        {
            case 0: handTransform.localPosition = offsetUp; break;
            case 1: handTransform.localPosition = offsetLeft; break;
            case 2: handTransform.localPosition = offsetDown; break;
            case 3: handTransform.localPosition = offsetRight; break;
        }

        if (handRenderer != null && bodyRenderer != null)
        {
            int baseOrder = bodyRenderer.sortingOrder;
            if (lookDir == 1 || lookDir == 2)
                handRenderer.sortingOrder = baseOrder + frontOffsetOrder;
            else
                handRenderer.sortingOrder = baseOrder + backOffsetOrder;
        }
    }

}
