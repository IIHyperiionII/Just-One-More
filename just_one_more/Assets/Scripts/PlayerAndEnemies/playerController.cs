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

    void Start()
    {
        PlayerData = GameManager.Instance.runtimePlayerData;
        Rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        GetMovementInput();
        GetAttackInput(); // Just for testing will be removed after weapons are implemented
        Attack();
    }

    void GetMovementInput()
    {
        input = new Vector2(0, 0);
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");
        if (input.magnitude > 1) input.Normalize();
    }

    void GetAttackInput()
    {
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
            GameObject bullet = Instantiate(bulletPrefab, transform.position, rotation);
            bullet.GetComponent<PlayerBulletControllerTest>().Initialize(PlayerData.bulletSpeed, PlayerData.damage);
        }
    }
    Quaternion UpdateAngle()
    {
        float distanceZ = Mathf.Abs(Camera.main.transform.position.z);
            mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceZ));
        Vector2 aimDirection = (mousePosition - transform.position).normalized;
        float tmpAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        Quaternion angle = Quaternion.Euler(0f, 0f, tmpAngle);
        return angle;
    }
    
    void FixedUpdate()
    {
        Vector2 movement = input * Time.deltaTime * PlayerData.moveSpeed;
        Rigidbody.MovePosition(Rigidbody.position + movement);
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
        PlayerData.coins += amount;
        Debug.Log("Coins: " + PlayerData.coins);
    }
}
