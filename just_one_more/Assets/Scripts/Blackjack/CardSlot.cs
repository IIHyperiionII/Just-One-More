using UnityEngine;
using UnityEngine.UI;

public class CardSlot : MonoBehaviour
{
    private Image image;
    private Vector3 originalPosition;

    public void Awake()
    {
        image = GetComponent<Image>();
        originalPosition = transform.position;
    }

    public void Initialize()
    {
        float randomRotationZ = Random.Range(-10, 10);
        float randomPositionY = Random.Range(-0.2f, 0.2f);
        float randomPositionX = Random.Range(-0.1f, 0.1f);
        transform.rotation = Quaternion.Euler(0f, 0f, randomRotationZ);
        transform.position = originalPosition + new Vector3(randomPositionX, randomPositionY, 0f);
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
