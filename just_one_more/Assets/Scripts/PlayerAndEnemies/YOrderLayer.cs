using UnityEngine;

public class YOrderLayer : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public BoxCollider2D positionReference;
    public CircleCollider2D circlePositionReference;
    public bool isCircle = false;

    void LateUpdate()
    {
        if (spriteRenderer == null) return;
        if (positionReference == null ){
            if (!isCircle){
            positionReference = this.gameObject.GetComponent<BoxCollider2D>();
            }
            else if (isCircle){
            circlePositionReference = this.gameObject.GetComponent<CircleCollider2D>();
            }
        }

        if (isCircle)
        {
            spriteRenderer.sortingOrder = Mathf.RoundToInt(-circlePositionReference.bounds.min.y * 100f);
        }
        else
        {
            spriteRenderer.sortingOrder = Mathf.RoundToInt(-positionReference.bounds.min.y * 100f);
        }
    }
}
