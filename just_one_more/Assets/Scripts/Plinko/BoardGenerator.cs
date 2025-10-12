using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BoardGenerator : MonoBehaviour
{
    [Header("Layout")]
    public int rows = 8;
    public float horizontalSpacing = 60f;
    public float verticalSpacing = 60f;
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
#if UNITY_EDITOR
        // Smaž staré pegs pokud existují
        for (int i = pegsContainer.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(pegsContainer.GetChild(i).gameObject);
        }
       
        // Vygeneruj nové
        for (int row = 0; row < rows; row++)
        {
            int pegsInRow = row + 3;
           
            for (int col = 0; col < pegsInRow; col++)
            {
                float x = (col - (pegsInRow - 1) / 2f) * horizontalSpacing;
                float y = -row * verticalSpacing;
               
                CreatePeg(new Vector2(x, y), row, col);
            }
        }
        
        Debug.Log($"Generated {pegsContainer.childCount} pegs as prefab instances!");
        #else
        Debug.LogWarning("Board generation only works in Editor mode!");
        #endif
    }
   
    GameObject CreatePeg(Vector2 position, int row, int col)
    {
        GameObject peg;
        
        #if UNITY_EDITOR
        // Vytvoř jako PREFAB INSTANCE (zůstane modrý)
        peg = (GameObject)PrefabUtility.InstantiatePrefab(pegPrefab, pegsContainer);
        #else
        // Fallback pro runtime (normální kopie)
        peg = Instantiate(pegPrefab, pegsContainer);
        #endif
        
        peg.name = $"Peg_{row}_{col}";
        
        // Nastav pozici a velikost
        RectTransform rt = peg.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.sizeDelta = new Vector2(pegSize, pegSize);
            rt.anchoredPosition = position;
        }
        
        // Uprav collider radius podle pegSize
        CircleCollider2D collider = peg.GetComponent<CircleCollider2D>();
        if (collider != null)
        {
            collider.radius = pegSize / 2f;
        }
        
        return peg;
    }
}
