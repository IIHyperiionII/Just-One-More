using UnityEngine;

public class HandFollowCursor : MonoBehaviour
{
    [Header("Rotation")]
    public float rotationSpeed = 15f;
    public float rotationOffset = 0f;

    [Header("Offsets (localSpace)")]
    public Vector3 offsetUp = new Vector3(-0.8f, 0.63f, 0f);
    public Vector3 offsetLeft = new Vector3(0f, 0.1f, 0f);
    public Vector3 offsetDown = new Vector3(0.68f, 0.63f, 0f);
    public Vector3 offsetRight = new Vector3(0f, 0.4f, 0f);

    [Header("Sorting")]
    public SpriteRenderer bodyRenderer;     
    public int frontOffsetOrder = 1;         
    public int backOffsetOrder = -1;        

    private Camera mainCam;
    private SpriteRenderer sr;

    void Start()
    {
        mainCam = Camera.main;
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (mainCam == null) return;
        Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = transform.position.z;
        Vector2 direction = mousePos - transform.position;
        if (direction.sqrMagnitude < 0.0001f) return;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + rotationOffset;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    public void ApplyLookDir(int lookDir)
    {
        switch (lookDir)
        {
            case 0: transform.localPosition = offsetUp; break;
            case 1: transform.localPosition = offsetLeft; break;
            case 2: transform.localPosition = offsetDown; break;
            case 3: transform.localPosition = offsetRight; break;
            default: break;
        }

        if (sr != null && bodyRenderer != null)
        {
            int baseOrder = bodyRenderer.sortingOrder;
            if (lookDir == 1 || lookDir == 2) 
                sr.sortingOrder = baseOrder + frontOffsetOrder;
            else
                sr.sortingOrder = baseOrder + backOffsetOrder;
        }
    }
}
