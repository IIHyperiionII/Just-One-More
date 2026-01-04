using UnityEngine;

// Interface for player bullet properties and behaviors
public interface IBulletPlayer
{
    int GetSpeed();
    int GetDamage();
    Quaternion GetInitialRotation();
    int GetFreezeLevel();
    int GetPiercingLevel();
    string GetSpriteName();
}
