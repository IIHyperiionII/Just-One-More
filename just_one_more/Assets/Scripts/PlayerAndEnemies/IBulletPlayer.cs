using UnityEngine;

public interface IBulletPlayer
{
    int GetSpeed();
    int GetDamage();
    Quaternion GetInitialRotation();
    int GetFreezeLevel();
    int GetPiercingLevel();
    string GetSpriteName();
}
