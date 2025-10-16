using UnityEngine;

[CreateAssetMenu(fileName = "BaseEnemiesData", menuName = "Scriptable Objects/BaseEnemiesData")]
public class EnemiesData : ScriptableObject
{
    public int hp = 10;
    public float moveSpeed = 5f;
    public int damage = 10;
    public float attackSpeed = 10f;
    public float bulletSpeed = 5f;
}
