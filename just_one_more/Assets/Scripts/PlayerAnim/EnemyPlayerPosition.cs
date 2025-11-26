using UnityEngine;

public class EnemyPlayerPosition : MonoBehaviour
{
    public string playerTag = "Player";

    public float leftRightValue;     // 1 = left, 0 = right (relative to player)
    public float moveTowardsValue;   // 1 if moving toward player, 0 from

    private Transform player;
    private Vector2 lastPosition;
    private Animator animator;

    private float currentSide = 0f;            // Stores last chosen side to prevent constant switching
    private const float switchThreshold = 0.4f; // Minimum distance before switching side 
    private const float holdThreshold = 0.1f;   // Dead zone where side stays same 

    void Start()
    {
        player = GameObject.FindGameObjectWithTag(playerTag)?.transform;
        animator = GetComponent<Animator>();
        lastPosition = transform.position;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        CalculateLeftRight();
        CalculateMovement();

        lastPosition = transform.position;
    }

    void Update()
    {
        SendToAnimator();
    }

    void CalculateLeftRight() // checks whether enemy is to the left or to the right of the player
    {
        float deltaX = transform.position.x - player.position.x;

        if (Mathf.Abs(deltaX) < holdThreshold)
        {
            leftRightValue = currentSide;
            return;
        }

        if (deltaX > switchThreshold)
        {
            currentSide = 1f;
        }
        else if (deltaX < -switchThreshold)
        {
            currentSide = 0f;
        }

        leftRightValue = currentSide;
    }

    void CalculateMovement()
    {
        Vector2 velocity = (Vector2)transform.position - lastPosition;
        Vector2 toPlayer = (player.position - transform.position).normalized;

        // Check if velocity is aligned with direction to player
        bool isMovingTowardPlayer = Vector2.Dot(velocity, toPlayer) > 0f;

        moveTowardsValue = isMovingTowardPlayer ? 1f : 0f;
    }

    void SendToAnimator()
    {
        if (animator == null) return;

        animator.SetFloat("LeftRight", leftRightValue);
        animator.SetFloat("MoveTowards", moveTowardsValue);
    }
}
