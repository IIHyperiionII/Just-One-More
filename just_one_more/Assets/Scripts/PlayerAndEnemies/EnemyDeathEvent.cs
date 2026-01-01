using UnityEngine;
using UnityEngine.Events;

public class EnemyDeathEvent : MonoBehaviour
{
    public UnityEvent onDeath;
    
    // Zavolej tuto metodu z TakeDamage() když HP <= 0
    public void TriggerDeathEvent()
    {
        onDeath?.Invoke();
    }
}
