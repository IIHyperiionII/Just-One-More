<<<<<<<< HEAD:just_one_more/Assets/Scripts/PlayerData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "Scriptable Objects/NewPlayerData")]
public class PlayerData : ScriptableObject
{
    public int hp = 100;
    public int moveSpeed = 5;
    public int attackSpeed = 20;
    public int damage = 10;
    public int money = 0;
    public int bulletSpeed = 10;
    public int knockback = 5;
    public bool isDead = false;
    public bool isMelee = false;

}
========
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "Scriptable Objects/NewPlayerData")]
public class PlayerData : ScriptableObject
{
    public int hp = 100;
    public int moveSpeed = 5;
    public int attackSpeed = 1;
    public int damage = 10;
    public int money = 0;
    public int bulletSpeed = 10;
    public bool isDead = false;

}
>>>>>>>> origin/main:just_one_more/Assets/Scripts/PlayerAndEnemies/PlayerData.cs
