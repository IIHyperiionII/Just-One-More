using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeController : MonoBehaviour
{
    public static ModeController Instance;
    public ModeAndWeaponSelection currentSelection;
    public GameObject darknessOverlay;
    public GameObject lightSource;
    public bool wasLoadedFromSave;
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
        if (lightSource == null && GameObject.FindGameObjectWithTag("Player") != null)
        {
            lightSource = GameObject.FindGameObjectWithTag("Player").transform.Find("Flashlight").gameObject;
        }
        if (currentSelection == null)
            {
                Debug.LogError("Current Selection ScriptableObject is not assigned.");
            }
        if (currentSelection.selectedMode == GameMode.nightRide && SceneManager.GetActiveScene().name == "tomScene")
        {
            darknessOverlay.SetActive(true);
            lightSource.SetActive(true);
        }
        else
        {
            if (darknessOverlay == null || lightSource == null) return;
            darknessOverlay.SetActive(false);
            lightSource.SetActive(false);
        }
    } 

    // Save and Load Methods
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
        currentSelection.basicDeifficultyCompleted = data.basicDifficultyCompleted;
        wasLoadedFromSave = true;
    }
    public void ResetSettingsToDefault()
    {
        currentSelection.selectedMode = GameMode.none;
        currentSelection.selectedWeapon = WeaponType.none;
        wasLoadedFromSave = false;
        darknessOverlay.SetActive(false);
        lightSource.SetActive(false);
    }
    
}
