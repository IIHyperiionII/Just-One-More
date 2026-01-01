#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class AddButtonSoundHelper : EditorWindow
{
    [MenuItem("Tools/Add ButtonSoundHelper to Scene Buttons")]
    static void AddToSceneButtons()
    {
        // Najdi všechny buttony ve scéně
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        
        int count = 0;
        foreach (Button button in allButtons)
        {
            // Přidej ButtonSoundHelper pokud tam ještě není
            if (button.GetComponent<ButtonSoundHelper>() == null)
            {
                button.gameObject.AddComponent<ButtonSoundHelper>();
                EditorUtility.SetDirty(button.gameObject);
                count++;
            }
        }
        
        Debug.Log($"ButtonSoundHelper přidán na {count} buttonů!");
    }
}
#endif
