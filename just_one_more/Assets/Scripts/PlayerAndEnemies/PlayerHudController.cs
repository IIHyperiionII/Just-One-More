using UnityEngine;
using TMPro;

public class PlayerHudController : MonoBehaviour
{
    [Header("References")]
    private PlayerData playerData;
    private GameObject gameManager;

    [Header("UI Elements")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI msText;
    public TextMeshProUGUI asText;

    void Start()
    {
        if (GameObject.FindGameObjectWithTag("GameController") == null)
        {
            Debug.LogWarning("PlayerHudController: GameManager reference is missing!");
        } else 
        {
            gameManager = GameObject.FindGameObjectWithTag("GameController");
            playerData = gameManager.GetComponent<GameManager>().runtimePlayerData;
        }
    }

    void Update()
    {
        if (GameModeManager.playerInCasino) return;
        if (playerData != null)
        {
            healthText.text = $"Health: {playerData.hp}";
            coinsText.text = $"Coins: {playerData.money}";
            msText.text = $"MS: {playerData.moveSpeed}";
            asText.text = $"AS: {playerData.attackSpeed}";
        }
    }
}
