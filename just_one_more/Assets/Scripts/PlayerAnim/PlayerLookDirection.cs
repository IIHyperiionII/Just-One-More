using UnityEngine;

public class PlayerLookDirection : MonoBehaviour
{
    public Animator animator;
    public Animator handAnimator;
    private Camera mainCam;
    public HandFollowCursor handScript; 

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        if (animator == null) return;

        Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = transform.position.z;

        Vector2 direction = mousePos - transform.position;

    
        if (direction.sqrMagnitude < 0.0001f) return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        // Updates look direction value to change the animation
        float lookDirValue = 0f;
        if (angle >= 45f && angle < 135f)
            lookDirValue = 0f; 
        else if (angle >= 135f && angle < 225f)
            lookDirValue = 1f;
        else if (angle >= 225f && angle < 315f)
            lookDirValue = 2f; 
        else
            lookDirValue = 3f; 

        animator.SetFloat("LookDir", lookDirValue);

        if (handAnimator != null)
            handAnimator.SetFloat("LookDir", lookDirValue);
        int lookDirInt = Mathf.RoundToInt(lookDirValue);
        if (handScript != null) handScript.ApplyLookDir(lookDirInt);

    }
}

