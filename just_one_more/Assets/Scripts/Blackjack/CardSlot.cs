using UnityEngine;

public class CardSlot : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Vector3 originalPosition;

    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalPosition = transform.position;
    }

    public void Initialize()
    {
        float randomRotationZ = Random.Range(-10, 10);
        float randomPositionY = Random.Range(-0.2f, 0.2f);
        float randomPositionX = Random.Range(-0.1f, 0.1f);
        transform.rotation = Quaternion.Euler(0f, 0f, randomRotationZ);
        transform.position = originalPosition + new Vector3(randomPositionX, randomPositionY, 0f);
        gameObject.SetActive(true);
    }

    public void ShowCard(Sprite cardSprite)
    {
        if (!cardSprite) return;

        spriteRenderer.sprite = cardSprite;
        gameObject.SetActive(true);
    }

    public void HideCard()
    {
        gameObject.SetActive(false);
    }
}
