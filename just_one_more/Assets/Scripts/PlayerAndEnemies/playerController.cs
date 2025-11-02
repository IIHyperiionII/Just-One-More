using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    private PlayerData PlayerData;
    private Rigidbody2D Rigidbody;
    private Vector2 input;
    private Vector3 mousePosition;
    public bool MouseKeyHoldDown = false;
    private float nextAttackTime = 0f;
    public GameObject bulletPrefab;
    public Vector2 MovementVector => input * PlayerData.moveSpeed;

    void Start()
    {
        PlayerData = GameManager.Instance.runtimePlayerData; // Access the runtime player data from GameManager
        Rigidbody = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        GetMovementInput();
        GetAttackInput(); // Just for testing will be removed after weapons are implemented
        Attack(); // Just for testing will be removed after weapons are implemented
    }

    void FixedUpdate()
    {
        Vector2 movement = input * Time.deltaTime * PlayerData.moveSpeed;
        Rigidbody.MovePosition(Rigidbody.position + movement);
    }

    void GetMovementInput()
    {
        input = new Vector2(0, 0);
        input.x = Input.GetAxis("Horizontal"); // Get horizontal input (A/D or Left/Right arrows)
        input.y = Input.GetAxis("Vertical"); // Get vertical input (W/S or Up/Down arrows)
        if (input.magnitude > 1) input.Normalize(); // Normalize to prevent faster diagonal movement
    }

    void GetAttackInput()
    {
        // Switch for when mouse button is held down
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            MouseKeyHoldDown = true;
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            MouseKeyHoldDown = false;
        }
    }
    void Attack()
    {
        if (MouseKeyHoldDown && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + 1f / PlayerData.attackSpeed;

            Quaternion rotation = UpdateAngle();
            GameObject bullet = Instantiate(bulletPrefab, transform.position, rotation); // Spawn bullet at player position with calculated rotation
            bullet.GetComponent<PlayerBulletControllerTest>().Initialize(PlayerData.bulletSpeed, PlayerData.damage); // Initialize bullet with player stats
        }
    }
    Quaternion UpdateAngle()
    {
        float distanceZ = Mathf.Abs(Camera.main.transform.position.z); // Distance from camera to player on Z axis
        mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceZ)); // Convert mouse position to world position
        Vector2 aimDirection = (mousePosition - transform.position).normalized; // Get normalized direction vector from player to mouse position
        float tmpAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg; // Calculate angle in degrees from radians
        Quaternion angle = Quaternion.Euler(0f, 0f, tmpAngle); // Create rotation quaternion from angle
        return angle;
    }

    public void takeDamage(int damage)
    {
        PlayerData.hp -= damage;
        if (PlayerData.hp <= 0) Die();
    }

    void Die()
    {
        PlayerData.isDead = true;
        Destroy(gameObject);
    }
    public void GetCoin(int amount)
    {
        PlayerData.money += amount;
    }
}
