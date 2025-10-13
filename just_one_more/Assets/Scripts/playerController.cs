using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    private PlayerData PlayerData;
    private Rigidbody2D Rigidbody;
    private Vector2 input;

    void Start()
    {
        PlayerData = GameManager.Instance.runtimePlayerData;
        Rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        input = new Vector2(0, 0);
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");
        if (input.magnitude > 1) input.Normalize();
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
        //TODO: add death amechanics
        PlayerData.isDead = true;
        Destroy(gameObject);
    }
}
