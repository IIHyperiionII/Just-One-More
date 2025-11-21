using UnityEngine;

public class PlayerColliderController : MonoBehaviour
{
    
    private BoxCollider2D boxColliderPlayer;
    private SpriteRenderer spriteRenderer;
    public GameObject doorCheckColliderGameObject;
    public GameObject wallCheckColliderGameObject;
    private BoxCollider2D doorCheckCollider;
    private BoxCollider2D wallCheckCollider;
    private Vector3 originalSpriteSize;
    private Vector3 originalColliderSize;
    private Vector2 originalDoorCheckColliderSize;
    private Vector2 originalWallCheckColliderSize;

    void Awake()
    {
        boxColliderPlayer = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        doorCheckCollider = doorCheckColliderGameObject.GetComponent<BoxCollider2D>();
        wallCheckCollider = wallCheckColliderGameObject.GetComponent<BoxCollider2D>();
        originalSpriteSize = spriteRenderer.bounds.size;
        originalColliderSize = boxColliderPlayer.size;
        originalDoorCheckColliderSize = doorCheckCollider.size;
        originalWallCheckColliderSize = wallCheckCollider.size;
    }

    void LateUpdate()
    {
        // Update collider size and offset to match sprite bounds
        Vector3 ratio = new Vector3(
            originalSpriteSize.x / spriteRenderer.bounds.size.x,
            originalSpriteSize.y / spriteRenderer.bounds.size.y,
            1f);
        boxColliderPlayer.size = new Vector2(
            originalColliderSize.x * ratio.x,
            originalColliderSize.y * ratio.y);
        doorCheckCollider.size = new Vector2(
            originalDoorCheckColliderSize.x * ratio.x,
            originalDoorCheckColliderSize.y * ratio.y);
        wallCheckCollider.size = new Vector2(
            originalWallCheckColliderSize.x * ratio.x,
            originalWallCheckColliderSize.y * ratio.y);
    }
}
