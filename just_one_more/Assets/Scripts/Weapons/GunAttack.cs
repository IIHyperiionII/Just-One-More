using UnityEngine;

public class GunAttack : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint; // (Volitelné) Místo, odkud kulka vylétá. Pokud prázdné, použije se střed hráče.

    // Reference na hlavní skript hráče
    private PlayerController playerController;

    void Start()
    {
        // Najdeme skript PlayerController na stejném objektu
        playerController = GetComponent<PlayerController>();

        // Pojistka, kdyby byl skript na rodiči nebo jinde
        if (playerController == null)
        {
            playerController = GetComponentInParent<PlayerController>();
        }

        if (playerController == null)
        {
            Debug.LogError("CHYBA: GunAttack nenašel PlayerController! Ujisti se, že jsou na stejném objektu.");
        }
    }

    void Update()
    {
        // Střelba levým tlačítkem myši
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (playerController == null) return;

        // 1. Získání rotace (směru míření) z PlayerControlleru
        // POZNÁMKA: V PlayerController.cs musí být metoda UpdateAngle nastavena jako 'public'!
        Quaternion rotation = playerController.UpdateAngle();

        // Určení pozice výstřelu (firePoint nebo pozice hráče)
        Vector3 spawnPos = (firePoint != null) ? firePoint.position : transform.position;

        // 2. Vytvoření kulky
        GameObject bulletObj = Instantiate(bulletPrefab, spawnPos, rotation);

        // 3. Získání tvého existujícího skriptu z kulky
        PlayerBulletControllerTest bulletScript = bulletObj.GetComponent<PlayerBulletControllerTest>();

        if (bulletScript != null)
        {
            // Výpočet poškození (damage * multiplier)
            int finalDamage = (int)(playerController.PlayerData.damage * playerController.multiplier);
            
            // Inicializace tvého skriptu
            bulletScript.Initialize(playerController.PlayerData.bulletSpeed, finalDamage);
        }
        else
        {
            Debug.LogWarning("Na objektu bulletPrefab chybí skript 'PlayerBulletControllerTest'!");
        }
    }
}