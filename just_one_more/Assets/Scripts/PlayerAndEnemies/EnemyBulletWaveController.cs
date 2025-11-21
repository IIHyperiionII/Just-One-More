using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEngine;

public class EnemyBulletWaveController : MonoBehaviour, IBullet
{
    private Vector2 waveMovement;
    private Vector2 motionForward;
    private Vector2 startPosition;
    private Vector2 direction;
    private Rigidbody2D Rigidbody;
    public string type = "WaveEnemyBullet";
    public int speed;
    public int damage;
    public Quaternion initialRotation;
    private float timeAlive = 0f;
    public int sign;
    private float frequency = 0f;
    private float amplitude = 0f;
    public string GetBulletType() { return type; }
    public Quaternion GetInitialRotation() { return initialRotation; }
    public int GetSpeed() { return speed; }
    public int GetDamage() { return damage; }
    public int GetSign() { return sign; }

    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        direction = transform.right; // applying given rotation to world x axis
        startPosition = Rigidbody.position;
        frequency = UnityEngine.Random.Range(5f, 15f); // random frequency for wave motion
        amplitude = UnityEngine.Random.Range(0.5f, 5f); // random amplitude for wave motion
    }

    public void Initialize(int bulletSpeed, int bulletDamage, int bulletSign, Quaternion rotation)
    {
        speed = bulletSpeed;
        damage = bulletDamage;
        sign = bulletSign;
        initialRotation = rotation;
    }
    void FixedUpdate()
    {
        if (GameModeManager.playerInCasino) return;
        GetWaveMovement();
            Rigidbody.MovePosition(motionForward + waveMovement);
    }
    void GetWaveMovement()
    {
        // calculate functional value of sine wave based on time alive
        timeAlive += Time.deltaTime;
        motionForward = direction * speed * timeAlive + startPosition; // position of linear forward motion
        float offset = Mathf.Sin(timeAlive * frequency) * amplitude; // offset from linear motion based on sine wave
        Vector2 sideAxis = new Vector2(-direction.y, direction.x); // perpendicular (90˚) axis to direction of travel, for wave motion
        waveMovement = sideAxis * offset * sign; // apply offset to side axis, multiplied by sign to determine left/right wave motion
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy")) return;
        if (other.gameObject.CompareTag("EnemyBullet")) return;
        if (other.gameObject.CompareTag("Coin")) return;
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().takeDamage(damage);
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Edge"))
        {
            Destroy(gameObject);
            return;
        }
    }

}
