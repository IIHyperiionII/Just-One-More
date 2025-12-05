using UnityEngine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;

[Serializable]
public class SaveData
{
    public int map;
    public int wave;
    public bool mapCompleted;
    public GameMode selectedMode;
    public WeaponType selectedWeapon;
    public bool basicDifficultyCompleted;
    public float time;
    public List<PlayerSaveData> players = new List<PlayerSaveData>();
    public List<EnemySaveData> enemies = new List<EnemySaveData>();
    public List<ProjectileSaveData> projectiles = new List<ProjectileSaveData>();

}

[Serializable]
public class PlayerSaveData
{
    public Vector3 position;
    public int hp;
    public int moveSpeed;
    public int attackSpeed;
    public int damage;
    public int money;
    public int bulletSpeed;
    public int knockback;
    public bool isDead;
    public bool isMelee;
    public int piercingLevel;
    public int dashLevel;
    public int hpRegenLevel;
    public int blockLevel;
    public int freezeLevel;
    public int needToGamble;
}

[Serializable]
public class EnemySaveData
{
    public Vector3 position;
    public string enemyType;
    public int hp;
    public int moveSpeed;
    public int damage;
    public int attackSpeed;
    public int bulletSpeed;
}

[Serializable]
public class ProjectileSaveData
{
    public Vector3 position;
    public string projectileType;
    public Quaternion initialRotation;
    public int speed;
    public int damage;
    public int sign; // for wave projectiles
}

