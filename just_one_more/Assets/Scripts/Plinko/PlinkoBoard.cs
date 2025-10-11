using UnityEngine;
using UnityEngine.UI;

public class PlinkoBoardGenerator : MonoBehaviour 
{
    [Header("Layout")]
    public int rows = 8;
    public float horizontalSpacing = 60f;
    public float verticalSpacing = 80f;
    public float pegSize = 20f;

    [Header("Appearance")]
    public Color pegColor = new Color(0.4f, 0.8f, 1f);

    [Header("PegPrefab")]
    public GameObject pegPrefab;
    
    [Header("Containers")]
    public Transform pegsContainer;
    
    [ContextMenu("Generate Board")]
    void GenerateBoard() 
    {
        // Smaž staré pegs pokud existují
        foreach (Transform child in pegsContainer) 
        {
            DestroyImmediate(child.gameObject);
        }
        
        // Vygeneruj nové
        for (int row = 0; row < rows; row++) 
        {
            int pegsInRow = row + 1;
            
            for (int col = 0; col < pegsInRow; col++) 
            {
                float x = (col - (pegsInRow - 1) / 2f) * horizontalSpacing;
                float y = -row * verticalSpacing;
                
                CreatePeg(new Vector2(x, y), row, col);
            }
        }
        
        Debug.Log($"Board vygenerován! Celkem {pegsContainer.childCount} pegů.");
    }
    
    GameObject CreatePeg(Vector2 position, int row, int col) 
    {
        GameObject peg;

        // Instantiate prefab (keeps Image, collider, any settings from prefab)
        peg = Instantiate(pegPrefab, pegsContainer);
        peg.name = $"Peg_{row}_{col}";

        // Ensure RectTransform settings (works for prefab or fallback)
        RectTransform rt = peg.GetComponent<RectTransform>();
        if (rt == null) rt = peg.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(pegSize, pegSize);
        rt.anchoredPosition = position;

        // Ensure CircleCollider2D exists and set radius (prefab can include this already)
        if (!peg.TryGetComponent<CircleCollider2D>(out var collider))
        {
            collider = peg.AddComponent<CircleCollider2D>();
        }
        collider.radius = pegSize / 2f;

        return peg;
    }
}
