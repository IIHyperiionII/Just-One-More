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
    }
    void Update()
    {
        if (currentSelection == null) return;
        if (currentSelection.selectedWeapon != WeaponType.none)
        {
            startButton.interactable = true;
        }
        else
        {
            startButton.interactable = false;
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("tomScene");
    } 
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
    public void SelectPistol()
    {
        currentSelection.selectedWeapon = WeaponType.Pistol;
        pistolButton.interactable = false;
        shotgunButton.interactable = true;
        meleeButton.interactable = true;
    }

    public void SelectShotgun()
    {
        currentSelection.selectedWeapon = WeaponType.Shotgun;
        pistolButton.interactable = true;
        shotgunButton.interactable = false;
        meleeButton.interactable = true;
    }

    public void SelectMelee()
    {
        currentSelection.selectedWeapon = WeaponType.Melee;
        pistolButton.interactable = true;
        shotgunButton.interactable = true;
        meleeButton.interactable = false;
    }

    public void SelectOneShot()
    {
        currentSelection.selectedMode = GameMode.OneShot;
        oneShotButton.interactable = false;
        nightRideButton.interactable = true;
        moneyLifeButton.interactable = true;
        noModeButton.interactable = true;
    }
    public void SelectNightRide()
    {
        currentSelection.selectedMode = GameMode.nightRide;
        oneShotButton.interactable = true;
        nightRideButton.interactable = false;
        moneyLifeButton.interactable = true;
    }
    public void SelectMoneyLife()
    {
        currentSelection.selectedMode = GameMode.MoneyLife;
        oneShotButton.interactable = true;
        nightRideButton.interactable = true;
        moneyLifeButton.interactable = false;
        noModeButton.interactable = true;
    }
    public void NoMode()
    {
        currentSelection.selectedMode = GameMode.none;

        oneShotButton.interactable = true;
        nightRideButton.interactable = true;
        moneyLifeButton.interactable = true;
        noModeButton.interactable = false;
    }
}
