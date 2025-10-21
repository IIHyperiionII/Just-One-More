using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "Scriptable Objects/NewPlayerData")]
public class PlayerData : ScriptableObject
{
    public int hp = 100;
    public int moveSpeed = 5;
    public int attackSpeed = 20;
    public int damage = 10;
    public int money = 0;
    public int attackModifier = 10;
    public bool isDead = false;
    public bool isMelee = false;

}
