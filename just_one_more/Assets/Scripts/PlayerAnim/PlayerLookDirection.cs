using UnityEngine;

public class PlayerLookDirection : MonoBehaviour
{
    [Header("Animators")]
    public Animator playerAnimator;
    public Animator handAnimator;

    [Header("Hand Script")]
    public HandFollowCursor handScript;

    private Camera mainCam;
    private PlayerController playerController;

    void Start()
    {
        mainCam = Camera.main;
        playerController = GetComponent<PlayerController>();

        if (handAnimator == null && transform.Find("HandAnchor/Hand") != null)
            handAnimator = transform.Find("HandAnchor/Hand").GetComponent<Animator>();
            

        if (handScript == null && transform.Find("HandAnchor") != null)
            handScript = transform.Find("HandAnchor").GetComponent<HandFollowCursor>();

        handAnimator.SetInteger("Weapon", 1);
    }

    void Update()
    {
        if (playerAnimator == null) return;

        if (playerController != null && playerController.isAttacking)
        {
            handAnimator.SetBool("isAttacking", true); 
            return;
        }
        else
        {
            handAnimator.SetBool("isAttacking", false);
        }

        Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = transform.position.z;
        Vector2 lookDirection = new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y);

        if (lookDirection.sqrMagnitude < 0.0001f) return;

        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        int lookDirInt = 0;
        if (angle >= 45f && angle < 135f) lookDirInt = 0; // Up
        else if (angle >= 135f && angle < 225f) lookDirInt = 1; // Left
        else if (angle >= 225f && angle < 315f) lookDirInt = 2; // Down
        else lookDirInt = 3; // Right

        float handSwitchFloat = 0f;
        if ((lookDirInt == 0 && angle > 90f) || (lookDirInt == 2 && angle > 270f))
            handSwitchFloat = 1f;

        playerAnimator.SetFloat("LookDir", lookDirInt);

        if (handAnimator != null)
        {
            handAnimator.SetFloat("LookDir", lookDirInt);
            handAnimator.SetFloat("HandSwitch", handSwitchFloat);
        }

        if (handScript != null)
        {
            handScript.ApplyLookDir(lookDirInt);
            handScript.SetHandSwitch(handSwitchFloat);
        }

        Vector2 moveVector = playerController != null ? playerController.MovementVector : Vector2.zero;
        bool isRunning = moveVector.sqrMagnitude > 0.001f;
        playerAnimator.SetBool("isRunning", isRunning);

        if (isRunning)
        {
            Vector2 moveDir = moveVector.normalized;
            Vector2 lookDirNorm = lookDirection.normalized;
            float runDot = Vector2.Dot(moveDir, lookDirNorm) >= 0f ? 1f : -1f;
            playerAnimator.SetFloat("runDirectionDot", runDot);
        }
    }
}
