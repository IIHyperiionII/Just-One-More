using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    private PlayerData PlayerData;
    private Rigidbody2D Rigidbody;

    void Start()
    {
        PlayerData = GameManager.Instance.runtimePlayerData;
        Rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        
    }

    void FixedUpdate()
    {
        Vector2 input = new Vector2(0, 0);
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");
        if (input.magnitude > 1) input.Normalize();
        Vector2 movement = input * Time.fixedDeltaTime * PlayerData.moveSpeed;
        Debug.Log("Player Movement: " + movement);
        Rigidbody.MovePosition(Rigidbody.position + movement / 2);
    }

    public void takeDamage(int damage)
    {
        PlayerData.hp -= damage;
        if (PlayerData.hp <= 0) Die();
    }

    void Die()
    {
        //TODO: add death amechanics
        PlayerData.isDead = true;
        Destroy(gameObject);
    }
}
