using UnityEngine;

public class PlinkoBall : MonoBehaviour
{
    [HideInInspector]
    public float betAmount;
    
    [HideInInspector]
    public PlinkoGame plinkoGame;
    
    private Rigidbody2D rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Malý náhodný impulz pro variabilitu
        float randomX = Random.Range(-0.3f, 0.3f);
        rb.linearVelocity = new Vector2(randomX, -5f);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bucket"))
        {
            Bucket bucket = other.GetComponent<Bucket>();
            float winAmount = betAmount * bucket.multiplier;
            
            plinkoGame.OnBallLanded(winAmount, bucket.multiplier);
            
            Destroy(gameObject);
        }
    }
    
    void Update()
    {
        // Safety - pokud kulička spadne moc dolů
        if (transform.position.y < -20f)
        {
            Debug.Log("Ball fell out of bounds!");
            plinkoGame.OnBallLanded(0, 0);
            Destroy(gameObject);
        }
    }
}
