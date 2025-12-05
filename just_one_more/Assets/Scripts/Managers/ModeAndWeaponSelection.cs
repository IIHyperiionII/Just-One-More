using UnityEngine;

[CreateAssetMenu(fileName = "ModAndWeaponSelection", menuName = "Scriptable Objects/ModAndWeaponSelection")]
public class ModeAndWeaponSelection : ScriptableObject
{
    public GameMode selectedMode;
    public WeaponType selectedWeapon;
    public bool basicDeifficultyCompleted;
}

public enum GameMode
{
    OneShot,
    nightRide,
    Inflation,
    none
    }

public enum WeaponType
{
    Pistol,
    Shotgun,
    Melee,
    none
}
