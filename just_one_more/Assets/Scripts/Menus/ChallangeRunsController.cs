using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChallangeRunsController : MonoBehaviour
{
    public ModeAndWeaponSelection currentSelection;
    public Button oneShotButton;
    public Button nightRideButton;
    public Button moneyLifeButton;
    public Button pistolButton;
    public Button shotgunButton;
    public Button meleeButton;
    public Button startButton;
    public Button mainMenuButton;
    public Button noModeButton;
    public Color original = new Color(1f, 1f, 1f, 1f);
    public Color disabled = new Color(0.5f, 0.5f, 0.5f, 1f);

    void Awake()
    {
        oneShotButton.onClick.AddListener(SelectOneShot);
        nightRideButton.onClick.AddListener(SelectNightRide);
        moneyLifeButton.onClick.AddListener(SelectMoneyLife);
        pistolButton.onClick.AddListener(SelectPistol);
        shotgunButton.onClick.AddListener(SelectShotgun);
        meleeButton.onClick.AddListener(SelectMelee);
        startButton.onClick.AddListener(StartGame);
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        noModeButton.onClick.AddListener(NoMode);
    }
    void Start()
    {
        if (currentSelection == null)
        {
            Debug.LogError("Current Selection ScriptableObject is not assigned.");
            return;
        }

        // Initialize buttons based on current selection
        currentSelection.selectedMode = GameMode.none;
        currentSelection.selectedWeapon = WeaponType.none;
        noModeButton.interactable = false;
        noModeButton.GetComponent<Image>().color = disabled;
    }
    void Update()
    {
        if (currentSelection == null) return;
        if (currentSelection.selectedWeapon != WeaponType.none)
        {
            startButton.interactable = true;
            startButton.GetComponent<Image>().color = original;
        }
        else
        {
            startButton.interactable = false;
            startButton.GetComponent<Image>().color = disabled;
        }
    }

    public void StartGame()
    {
        ModeController.Instance.currentSelection = currentSelection;
        Debug.Log("Selected Mode: " + currentSelection.selectedMode);
        Debug.Log("Selected Weapon: " + currentSelection.selectedWeapon);
        FindFirstObjectByType<SceneLoader>().LoadGameplayScene();
    } 
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
    public void SelectPistol()
    {
        currentSelection.selectedWeapon = WeaponType.Pistol;
        pistolButton.interactable = false;
        pistolButton.GetComponent<Image>().color = disabled;
        shotgunButton.interactable = true;
        shotgunButton.GetComponent<Image>().color = original;
        meleeButton.interactable = true;
        meleeButton.GetComponent<Image>().color = original;
    }

    public void SelectShotgun()
    {
        currentSelection.selectedWeapon = WeaponType.Shotgun;
        pistolButton.interactable = true;
        pistolButton.GetComponent<Image>().color = original;
        shotgunButton.interactable = false;
        shotgunButton.GetComponent<Image>().color = disabled;
        meleeButton.interactable = true;
        meleeButton.GetComponent<Image>().color = original;
    }

    public void SelectMelee()
    {
        currentSelection.selectedWeapon = WeaponType.Melee;
        pistolButton.interactable = true;
        pistolButton.GetComponent<Image>().color = original;
        shotgunButton.interactable = true;
        shotgunButton.GetComponent<Image>().color = original;
        meleeButton.interactable = false;
        meleeButton.GetComponent<Image>().color = disabled;
    }

    public void SelectOneShot()
    {
        currentSelection.selectedMode = GameMode.OneShot;
        oneShotButton.interactable = false;
        oneShotButton.GetComponent<Image>().color = disabled;
        nightRideButton.interactable = true;
        nightRideButton.GetComponent<Image>().color = original;
        moneyLifeButton.interactable = true;
        moneyLifeButton.GetComponent<Image>().color = original;
        noModeButton.interactable = true;
        noModeButton.GetComponent<Image>().color = original;
    }
    public void SelectNightRide()
    {
        currentSelection.selectedMode = GameMode.nightRide;
        oneShotButton.interactable = true;
        oneShotButton.GetComponent<Image>().color = original;
        nightRideButton.interactable = false;
        nightRideButton.GetComponent<Image>().color = disabled;
        moneyLifeButton.interactable = true;
        moneyLifeButton.GetComponent<Image>().color = original;
        noModeButton.interactable = true;
        noModeButton.GetComponent<Image>().color = original;
    }
    public void SelectMoneyLife()
    {
        currentSelection.selectedMode = GameMode.MoneyLife;
        oneShotButton.interactable = true;
        oneShotButton.GetComponent<Image>().color = original;
        nightRideButton.interactable = true;
        nightRideButton.GetComponent<Image>().color = original;
        moneyLifeButton.interactable = false;
        moneyLifeButton.GetComponent<Image>().color = disabled;
        noModeButton.interactable = true;   
        noModeButton.GetComponent<Image>().color = original;
    }
    public void NoMode()
    {
        currentSelection.selectedMode = GameMode.none;

        oneShotButton.interactable = true;
        oneShotButton.GetComponent<Image>().color = original;
        nightRideButton.interactable = true;
        nightRideButton.GetComponent<Image>().color = original;
        moneyLifeButton.interactable = true;
        moneyLifeButton.GetComponent<Image>().color = original;
        noModeButton.interactable = false;
        noModeButton.GetComponent<Image>().color = disabled;
    }
}
