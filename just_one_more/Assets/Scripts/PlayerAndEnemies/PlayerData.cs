using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "Scriptable Objects/NewPlayerData")]
public class PlayerData : ScriptableObject
{
    public int hp = 100;
    public float moveSpeed = 5f;
    public float attackSpeed = 1f;
    public int damage = 10;
    public int money = 0;
    public float bulletSpeed = 10f;
    public bool isDead = false;
    public int coins = 0;

}
