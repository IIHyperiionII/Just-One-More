using UnityEngine;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    private PlayerData playerData; // runtime data

    // Přetáhneš sem texty z panelu
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI dmgText;
    public TextMeshProUGUI moneyText;

    void Start()
    {
        // Napojíme na runtimePlayerData z GameManageru
        if (GameManager.Instance != null)
        {
            playerData = GameManager.Instance.runtimePlayerData;
        }
    }

    void Update()
    {
        if (playerData == null) return;

        // Aktualizace UI textů podle aktuálních hodnot
        healthText.text = $"HP: {playerData.hp}";
        speedText.text = $"Speed: {playerData.moveSpeed}";
        dmgText.text = $"DMG: {playerData.damage}";
        moneyText.text = $"Money: {playerData.money}";
    }
}
