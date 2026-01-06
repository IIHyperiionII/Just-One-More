using UnityEngine;

// Interface for bullet properties and behaviors
public interface IBullet
{
    string GetBulletType();
    Quaternion GetInitialRotation();
    int GetSpeed();
    int GetDamage();
    int GetSign();
    string GetSpriteName();
}
