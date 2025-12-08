using UnityEngine;
using UnityEngine.UI; // Potřebné pro práci s UI

public class WeaponSelectionMenu : MonoBehaviour
{
    [Header("References")]
    public PlayerController playerController; // Odkaz na skript hráče
    public GameObject menuPanel; // Odkaz na Panel, který obsahuje tlačítka

    void Start()
    {
        // Okamžitě při startu zastavíme čas, aby se nic nehýbalo
        Time.timeScale = 0f;
        
        // Ujistíme se, že menu je vidět
        if (menuPanel != null)
            menuPanel.SetActive(true);
        else
            Debug.LogError("Není přiřazen Menu Panel v inspektoru!");
    }

    // Tuto metodu přiřadíme tlačítku pro STŘELNOU ZBRAŇ
    public void SelectGun()
    {
        Debug.Log("Vybrána zbraň: GUN");
        if (playerController != null)
        {
            playerController.gun = true;
        }
        StartGame();
    }

    // Tuto metodu přiřadíme tlačítku pro MELEE (nebo jinou volbu)
    public void SelectMelee()
    {
        Debug.Log("Vybrána zbraň: MELEE");
        if (playerController != null)
        {
            playerController.gun = false;
        }
        StartGame();
    }

    void StartGame()
    {
        // Schováme menu
        menuPanel.SetActive(false);
        
        // Rozběhneme čas ve hře
        Time.timeScale = 1f;
    }
}