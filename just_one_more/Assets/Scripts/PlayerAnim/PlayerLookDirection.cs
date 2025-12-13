using UnityEngine;
using UnityEngine.XR;

public class PlayerLookDirection : MonoBehaviour
{
    [Header("Animators")]
    public Animator playerAnimator; 
    public Animator handAnimator;   

    [Header("Hand Script")]
    public HandFollowCursor handScript; 

    private Camera mainCam;                
    private PlayerController playerController;
    private ModeAndWeaponSelection currentSelection;

    void Start()
    {
        mainCam = Camera.main; 
        playerController = GetComponent<PlayerController>(); 

        if (handAnimator == null && transform.Find("HandAnchor/Hand") != null)
            handAnimator = transform.Find("HandAnchor/Hand").GetComponent<Animator>();

        if (handScript == null && transform.Find("HandAnchor") != null)
            handScript = transform.Find("HandAnchor").GetComponent<HandFollowCursor>();

        if (currentSelection == null)
        {
            currentSelection = ModeController.Instance.currentSelection;
        }
        switch (currentSelection.selectedWeapon)
        {
            case WeaponType.Melee:
                handAnimator.SetInteger("Weapon", 1);
                break;
            case WeaponType.Pistol:
                handAnimator.SetInteger("Weapon", 2);
                break;
            case WeaponType.Shotgun:
                handAnimator.SetInteger("Weapon", 3);
                break;
            default:
                break;

        }
    }

    private void OnEnable() // used after returning from casino
    {
        if (currentSelection == null)
        {
            if (ModeController.Instance == null) return;
            currentSelection = ModeController.Instance.currentSelection;
        }
        switch (currentSelection.selectedWeapon)
        {
            case WeaponType.Melee:
                handAnimator.SetInteger("Weapon", 1);
                break;
            case WeaponType.Pistol:
                handAnimator.SetInteger("Weapon", 2);
                break;
            case WeaponType.Shotgun:
                handAnimator.SetInteger("Weapon", 3);
                break;
            default:
                break;

        }
    }
    void Update()
    {
        if (GameModeManager.timeIsPaused) return;
        if (playerAnimator == null) return;

        // Handle attacking animation
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
        if (lookDirection.sqrMagnitude < 0.0001f) return; // Ignore if very small

        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f; // Ensure angle is positive

        // Determine main look direction (up, left, down, right)
        int lookDirInt = 0;
        if (angle >= 45f && angle < 135f) lookDirInt = 0; // Up
        else if (angle >= 135f && angle < 225f) lookDirInt = 1; // Left
        else if (angle >= 225f && angle < 315f) lookDirInt = 2; // Down
        else lookDirInt = 3; // Right

        float handSwitchFloat = 0f;

        if (lookDirInt == 0) // Up
        {
            if (angle < 75f) handSwitchFloat = 0f;
            else if (angle < 105f) handSwitchFloat = 1f;
            else handSwitchFloat = 2f;
        }
        else if (lookDirInt == 2) // Down
        {
            if (angle < 255f) handSwitchFloat = 0f;
            else if (angle < 285f) handSwitchFloat = 1f;
            else handSwitchFloat = 2f;
        }
        else
        {
            handSwitchFloat = 0f; // Left or right
        }

        // Update player animator
        playerAnimator.SetFloat("LookDir", lookDirInt);

        // Update hand animator
        if (handAnimator != null)
        {
            handAnimator.SetFloat("LookDir", lookDirInt);
            handAnimator.SetFloat("HandSwitch", handSwitchFloat);
        }

        // Update hand script
        if (handScript != null)
        {
            handScript.ApplyLookDir(lookDirInt);
            handScript.SetHandSwitch(handSwitchFloat);
        }

        Vector2 moveVector = playerController != null ? playerController.MovementVector : Vector2.zero;
        bool isRunning = moveVector.sqrMagnitude > 0.001f;
        playerAnimator.SetBool("isRunning", isRunning); // Set running state

        // Set run direction dot to adjust run animation
        if (isRunning)
        {
            Vector2 moveDir = moveVector.normalized;
            Vector2 lookDirNorm = lookDirection.normalized;
            float runDot = Vector2.Dot(moveDir, lookDirNorm) >= 0f ? 1f : -1f;
            playerAnimator.SetFloat("runDirectionDot", runDot);
        }
    }
}
