using UnityEngine;
using UnityEngine.Animations;

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

    [Header("Hand flip/switch")]
    public float handSwitch = 0f;

    private Camera mainCam;
    private PlayerController playerController;

    private Quaternion lastRotation;
    private int currentLookDir = 3;

    public SpriteRenderer spriteRenderer;
    public BoxCollider2D positionReference;
    public BoxCollider2D positionReference2;

    void Start()
    {
        mainCam = Camera.main;
        playerController = GetComponentInParent<PlayerController>();

        if (handTransform == null) handTransform = transform.GetChild(0);
        if (handRenderer == null && handTransform != null)
            handRenderer = handTransform.GetComponent<SpriteRenderer>();

        lastRotation = handTransform.rotation;
    }

    void Update()
    {
        if (GameModeManager.timeIsPaused) return;

        // Useless ???
        if (Time.timeScale == 0f) return;
        
        if (mainCam == null || handTransform == null) return;

        if (playerController != null && playerController.isAttacking)
        {
            Quaternion targetRotation;
            
            if (currentLookDir == 0)         // Up
               targetRotation = Quaternion.Euler(0f, 0f, 250f);
            else if (currentLookDir == 1)    // Left
                targetRotation = Quaternion.Euler(0f, 0f, 270f);
            else if (currentLookDir == 2)    // Down
                targetRotation = Quaternion.identity;
            else                             // Right
                targetRotation = Quaternion.Euler(0f, 0f, 180f);

            handTransform.rotation = Quaternion.Lerp(handTransform.rotation, targetRotation, Time.deltaTime * 10f);
            lastRotation = handTransform.rotation;
            return;
        }

        Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = handTransform.position.z;

        Vector2 direction = mousePos - handTransform.position;
        if (direction.sqrMagnitude < 0.0001f) return;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + rotationOffset;
        Quaternion targetRotationMouse = Quaternion.Euler(0f, 0f, targetAngle);
        handTransform.rotation = Quaternion.Lerp(lastRotation, targetRotationMouse, Time.deltaTime * rotationSpeed);

        lastRotation = handTransform.rotation;
    }

    public void ApplyLookDir(int lookDir)
    {
        if (handTransform == null) return;
        currentLookDir = lookDir;

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
            if (lookDir == 1 || lookDir == 2){
                if (positionReference2 == null){
                    positionReference2 = this.gameObject.GetComponent<BoxCollider2D>();
                }
                spriteRenderer.sortingOrder = Mathf.RoundToInt(-positionReference2.bounds.min.y * 100f);
            } else{
                if (positionReference == null){
                    positionReference = this.gameObject.GetComponent<BoxCollider2D>();
                }
                spriteRenderer.sortingOrder = Mathf.RoundToInt(-positionReference.bounds.min.y * 100f);
            }
        }
    }

    public void SetHandSwitch(float state)
    {
        handSwitch = state;
    }


}
