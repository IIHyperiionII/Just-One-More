using UnityEngine;

[CreateAssetMenu(fileName = "ModAndWeaponSelection", menuName = "Scriptable Objects/ModAndWeaponSelection")]
public class ModeAndWeaponSelection : ScriptableObject
{
    public GameMode selectedMode = GameMode.none;
    public WeaponType selectedWeapon;
    public bool basicDeifficultyCompleted;
}

public enum GameMode
{
    OneShot,
    nightRide,
    MoneyLife,
    none
    }

public enum WeaponType
{
    Pistol,
    Shotgun,
    Melee,
    none
}
