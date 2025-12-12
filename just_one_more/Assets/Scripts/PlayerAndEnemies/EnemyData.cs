using UnityEngine;

[CreateAssetMenu(fileName = "BaseEnemyData", menuName = "Scriptable Objects/BaseEnemyData")]
public class EnemyData : ScriptableObject
{
    public string type = "";
    public int hp = 10;
    public int moveSpeed = 5;
    public int damage = 10;
    public int attackSpeed = 10;
    public int bulletSpeed = 5;
    public int value = 1;
}
