using UnityEngine;

// IEnemy.cs
public interface IBullet
{
    string GetBulletType();
    Quaternion GetInitialRotation();
    int GetSpeed();
    int GetDamage();
    int GetSign();
    string GetSpriteName();
}
