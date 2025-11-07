using UnityEngine;

public class PlayerLookDirection : MonoBehaviour
{
    public Animator animator;
    public Animator handAnimator;
    public HandFollowCursor handScript;

    private Camera mainCam;
    private PlayerController playerController;

    void Start()
    {
        mainCam = Camera.main;
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (animator == null) return;

        Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = transform.position.z;
        Vector2 lookDirection = (mousePos - transform.position);
        if (lookDirection.sqrMagnitude < 0.0001f) return;

        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        float lookDirValue = 0f;
        if (angle >= 45f && angle < 135f)
            lookDirValue = 0f;
        else if (angle >= 135f && angle < 225f)
            lookDirValue = 1f;
        else if (angle >= 225f && angle < 315f)
            lookDirValue = 2f;
        else
            lookDirValue = 3f;

        int lookDirInt = Mathf.RoundToInt(lookDirValue);

        animator.SetFloat("LookDir", Mathf.Round(lookDirValue));
        if (handAnimator != null)
            handAnimator.SetFloat("LookDir", lookDirValue);
        if (handScript != null)
            handScript.ApplyLookDir(lookDirInt);

        Vector2 moveVector = playerController != null ? playerController.MovementVector : Vector2.zero;
        bool isRunning = moveVector.sqrMagnitude > 0.001f;
        animator.SetBool("isRunning", isRunning);
        
        if (isRunning)
        {
            Vector2 moveDir = moveVector.normalized;
            Vector2 lookDirNorm = lookDirection.normalized;
            float runDot = Vector2.Dot(moveDir, lookDirNorm) >= 0f ? 1f : -1f;
            animator.SetFloat("runDirectionDot", runDot);
        }
    }
}
