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
        // Movement need to be in FixedUpdate when using Rigidbody to work with physics correctly
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(moveX, moveY) * Time.deltaTime * PlayerData.moveSpeed;
        Rigidbody.MovePosition(Rigidbody.position + movement);
    }
    
    void takeDamage(int damage)
    {
        PlayerData.hp -= damage;
        if (PlayerData.hp < 0)
        {
            Die();
        }
    }

    void Die()
    {
        //TODO: add death amechanics
        Destroy(gameObject);
    }
}
