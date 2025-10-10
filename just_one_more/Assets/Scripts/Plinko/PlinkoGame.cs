using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlinkoGame : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject ballPrefab;
    
    [Header("Settings")]
    public Vector2 ballSpawnPosition = new Vector2(0, 6.5f);
    public float testBetAmount = 50f;
    
    [Header("UI References")]
    public Button dropBallButton;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI betInfoText;
    
    private bool ballInPlay = false;
    
    void Start()
    {
        dropBallButton.onClick.AddListener(DropBall);
        
        betInfoText.text = $"Bet: ${testBetAmount:F0}";
        resultText.text = "Press DROP BALL to play";
    }
    
    public void DropBall()
    {
        if (ballInPlay)
        {
            Debug.Log("Ball already in play!");
            return;
        }
        
        GameObject ball = Instantiate(ballPrefab, ballSpawnPosition, Quaternion.identity);
        PlinkoBall ballScript = ball.AddComponent<PlinkoBall>();
        ballScript.betAmount = testBetAmount;
        ballScript.plinkoGame = this;
        
        ballInPlay = true;
        dropBallButton.interactable = false;
        resultText.text = "Ball in play...";
        resultText.color = Color.white;
    }
    
    public void OnBallLanded(float winAmount, float multiplier)
    {
        ballInPlay = false;
        dropBallButton.interactable = true;
        
        if (winAmount > testBetAmount)
        {
            resultText.text = $"<color=green>WON ${winAmount:F0}!</color> ({multiplier}x)";
            resultText.color = Color.green;
        }
        else if (winAmount < testBetAmount)
        {
            resultText.text = $"<color=red>Lost ${testBetAmount - winAmount:F0}</color> ({multiplier}x)";
            resultText.color = Color.red;
        }
        else
        {
            resultText.text = $"Break even ({multiplier}x)";
            resultText.color = Color.yellow;
        }
        
        Debug.Log($"Ball landed! Bet: ${testBetAmount}, Won: ${winAmount}, Multiplier: {multiplier}x");
    }
}
