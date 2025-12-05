using UnityEngine;

public class ModeController : MonoBehaviour
{
    public static ModeController Instance;
    public ModeAndWeaponSelection currentSelection;
    public GameObject darknessOverlay;
    public GameObject lightSource;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (currentSelection == null)
            {
                Debug.LogError("Current Selection ScriptableObject is not assigned.");
            }
        if (currentSelection.selectedMode == GameMode.nightRide)
        {
            darknessOverlay.SetActive(true);
            lightSource.SetActive(true);
        }
        else
        {
            darknessOverlay.SetActive(false);
            lightSource.SetActive(false);
        }
    } 

    public void GetSaveData()
    {
        SaveData data = SaveSystem.Instance.currentSaveData;
        data.selectedMode = currentSelection.selectedMode;
        data.selectedWeapon = currentSelection.selectedWeapon;
        data.basicDifficultyCompleted = currentSelection.basicDeifficultyCompleted;
    }

    public void ApplySaveData()
    {
        SaveData data = SaveSystem.Instance.currentSaveData;
        currentSelection.selectedMode = data.selectedMode;
        currentSelection.selectedWeapon = data.selectedWeapon;
    }
    
}
