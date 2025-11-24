using UnityEngine;

public class YOrderLayer : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public BoxCollider2D positionReference;

    void LateUpdate()
    {
        if (spriteRenderer == null) return;
        if (positionReference == null)
            positionReference = this.gameObject.GetComponent<BoxCollider2D>();
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-positionReference.bounds.min.y * 100f);
    }
}
