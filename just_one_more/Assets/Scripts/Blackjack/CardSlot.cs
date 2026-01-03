using UnityEngine;
using UnityEngine.UI;

// Card slot for sprites on the table

public class CardSlot : MonoBehaviour
{
    private Image image;

    private RectTransform rectTransform;
    private Vector2 originalAnchoredPosition;

    public void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        originalAnchoredPosition = rectTransform.anchoredPosition;
    }

    public void Initialize()
    {
        float randomRotationZ = Random.Range(-10, 10);
        float randomPositionY = Random.Range(-20f, 20f);
        float randomPositionX = Random.Range(-10f, 10f);
        rectTransform.rotation = Quaternion.Euler(0f, 0f, randomRotationZ);
        rectTransform.anchoredPosition = originalAnchoredPosition + new Vector2(randomPositionX, randomPositionY);
        gameObject.SetActive(false);
    }

    public void ShowCard(Sprite cardSprite)
    {
        if (!cardSprite) return;

        image.sprite = cardSprite;
        gameObject.SetActive(true);
    }

    public void HideCard()
    {
        gameObject.SetActive(false);
    }
}
