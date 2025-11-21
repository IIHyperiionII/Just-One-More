using System;
using UnityEngine;

public class BestTimeSaveData
{
    [Serializable]
    public class BestTimeData
    {
        public float bestTime;

        public GameMode selectedMode;
        public WeaponType selectedWeapon;
    }
}
