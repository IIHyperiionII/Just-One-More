using UnityEngine;

public class CoinController : MonoBehaviour
{
    int value;

    public void SetValue(int val)
    {
        Debug.Log("Setting coin value to: " + val);
        value = val;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player collected coin worth: " + value);
            other.gameObject.GetComponent<PlayerController>().GetCoin(value);
            Destroy(gameObject);
        }
    }
}
