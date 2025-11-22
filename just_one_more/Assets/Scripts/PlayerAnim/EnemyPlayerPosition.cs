using UnityEngine;

public class EnemyPlayerPosition : MonoBehaviour
{
    public string playerTag = "Player";

    public float leftRightValue;
    public float moveTowardsValue;

    private Transform player;
    private Vector2 lastPosition;
    private Animator animator;

    private float currentSide = 0f;
    private const float switchThreshold = 0.4f;
    private const float holdThreshold = 0.1f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag(playerTag)?.transform;
        animator = GetComponent<Animator>();
        lastPosition = transform.position;
    }

    void Update()
    {
        if (player == null) return;

        CalculateLeftRight();
        CalculateMovement();
        SendToAnimator();
    }

    void CalculateLeftRight()
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

        bool isMovingTowardPlayer = Vector2.Dot(velocity, toPlayer) > 0f;

        moveTowardsValue = isMovingTowardPlayer ? 1f : 0f;

        lastPosition = transform.position;
    }

    void SendToAnimator()
    {
        if (animator == null) return;

        animator.SetFloat("LeftRight", leftRightValue);
        animator.SetFloat("MoveTowards", moveTowardsValue);
    }
}
