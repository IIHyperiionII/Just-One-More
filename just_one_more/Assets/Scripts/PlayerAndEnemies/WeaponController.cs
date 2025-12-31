using UnityEngine;


public class WeaponController : MonoBehaviour
{
    [Header("Stats")]
    public float startTimeBtwAttack;
    public float attackRange = 1.8f;

    [Header("Configuration")]
    public LayerMask IsEnemy;
    
    // 180 = Half Circle, 90 = Triangle/Cone
    [Range(0, 360)]
    public float attackAngle = 120f; 
    private Vector3 mousePosition;
    public Quaternion mouseDirection;
    public GameObject bulletPrefab;
    public Sprite pistolBulletSprite;
    public Sprite shotgunBulletSprite;

    void Update()
    {
        mouseDirection = UpdateAngle();
    }
    public void AttackSword(int damage, float knockback)
    {

        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(transform.position, attackRange, IsEnemy);
        
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            if (enemiesToDamage[i].tag != "Enemy") continue;
            Vector2 directionToEnemy = (enemiesToDamage[i].transform.position - transform.position).normalized;
            
            float angleToEnemy = Vector2.Angle(mouseDirection * Vector3.right, directionToEnemy);

            if (angleToEnemy < attackAngle / 2f)
            {
                enemiesToDamage[i].GetComponent<IEnemy>().TakeDamage(damage);
                enemiesToDamage[i].GetComponent<IEnemy>().Knockback(knockback);
            }
        }
    }

    public void AttackGun(int bulletSpeed, int damage, int piercingLevel, int freezeLevel)
    {
        Quaternion rotation = UpdateAngle();
        GameObject bullet = Instantiate(bulletPrefab, transform.position + mouseDirection * Vector3.right * 0.5f, rotation); // Spawn bullet at player position with calculated rotation
        bullet.GetComponent<PlayerBulletController>().Initialize(bulletSpeed, damage, piercingLevel, freezeLevel, rotation, pistolBulletSprite, true); // Initialize bullet with player stats
        bullet.transform.SetParent(GameObject.FindGameObjectWithTag("BulletsPlayerParent").transform); // Set the parent of the spawned bullet for organization
    }

    public void AttackShotgun(int bulletSpeed, int damage , int piercingLevel, int freezeLevel)
    {
        Quaternion rotation = UpdateAngle();
        GameObject bullet = Instantiate(bulletPrefab, transform.position + mouseDirection * Vector3.right * 0.5f, rotation); // Spawn bullet at player position with calculated rotation
        bullet.GetComponent<PlayerBulletController>().Initialize(bulletSpeed, damage, piercingLevel, freezeLevel, rotation, shotgunBulletSprite, false); // Initialize bullet with player stats
        GameObject bullet2 = Instantiate(bulletPrefab, transform.position + mouseDirection * Vector3.right * 0.5f, rotation * Quaternion.Euler(0, 0, 20)); // Spawn bullet at player position with calculated rotation
        bullet2.GetComponent<PlayerBulletController>().Initialize(bulletSpeed, damage, piercingLevel, freezeLevel, rotation * Quaternion.Euler(0, 0, 20), shotgunBulletSprite, false); // Initialize bullet with player stats
        GameObject bullet3 = Instantiate(bulletPrefab, transform.position + mouseDirection * Vector3.right * 0.5f, rotation * Quaternion.Euler(0, 0, -20)); // Spawn bullet at player position with calculated rotation
        bullet3.GetComponent<PlayerBulletController>().Initialize(bulletSpeed, damage, piercingLevel, freezeLevel, rotation * Quaternion.Euler(0, 0, -20), shotgunBulletSprite, false); // Initialize bullet with player stats
        bullet.transform.SetParent(GameObject.FindGameObjectWithTag("BulletsPlayerParent").transform); // Set the parent of the spawned bullet for organization
        bullet2.transform.SetParent(GameObject.FindGameObjectWithTag("BulletsPlayerParent").transform); // Set the parent of the spawned bullet for organization
        bullet3.transform.SetParent(GameObject.FindGameObjectWithTag("BulletsPlayerParent").transform); // Set the parent of the spawned bullet for organization
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

    void OnDrawGizmos()
    {
        if (mouseDirection == null) return;
        Gizmos.color = new Color(1, 1, 0, 0.3f); 
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blueViolet;
        Gizmos.DrawWireSphere(transform.position + mouseDirection * Vector3.right * 0.5f, 0.2f);

        Gizmos.color = Color.red;
        
        Vector3 direction = mouseDirection * Vector3.right;
        float halfAngle = attackAngle / 2f;

        Vector3 topDir = Quaternion.Euler(0, 0, halfAngle) * direction;
        Vector3 botDir = Quaternion.Euler(0, 0, -halfAngle) * direction;
        Gizmos.DrawLine(transform.position, transform.position + topDir * attackRange);
        Gizmos.DrawLine(transform.position, transform.position + botDir * attackRange);
    }
}
