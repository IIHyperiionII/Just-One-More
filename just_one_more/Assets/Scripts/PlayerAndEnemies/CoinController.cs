using UnityEngine;

public class CoinController : MonoBehaviour, ICoin
{
    int value;

    void Start()
    {
      this.transform.SetParent(GameObject.FindGameObjectWithTag("MoneyParent").transform);  
    }
    public void SetValue(int val)
    {
        value = val;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider belongs to the player
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player collected coin worth: " + value);
            other.gameObject.GetComponent<PlayerController>().GetCoin(value);
            Destroy(gameObject);
        }
    }
    public int GetValue()
    {
        return value;
    }
}
