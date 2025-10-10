using UnityEngine;
using TMPro;

public class Bucket : MonoBehaviour
{
    public float multiplier = 1.0f;
    public Color bucketColor = Color.green;

    void Start()
    {
        GetComponent<SpriteRenderer>().color = bucketColor;

        CreateMultiplierText();
    }

    void CreateMultiplierText()
    {
        GameObject textObj = new GameObject("MultiplierText");
        textObj.transform.SetParent(transform);
        textObj.transform.localPosition = new Vector3(0, 1.2f, 0);
        
        TextMeshPro tmp = textObj.AddComponent<TextMeshPro>();
        tmp.text = multiplier + "x";
        tmp.fontSize = 3;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        
        RectTransform rt = textObj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(2, 1);
    }
}
