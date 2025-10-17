using UnityEngine;

[CreateAssetMenu(fileName = "BaseEnemyData", menuName = "Scriptable Objects/BaseEnemyData")]
public class EnemyData : ScriptableObject
{
    public int hp = 10;
    public float moveSpeed = 5f;
    public int damage = 10;
    public float attackSpeed = 10f;
    public float bulletSpeed = 5f;
}
