using UnityEngine;

public static class EnemySoundHelper
{
    public static void PlayDeathSound(string enemyType)
    {
        if (SoundController.Instance == null)
        {
            Debug.LogWarning("SoundController.Instance is null!");
            return;
        }
        
        switch (enemyType)
        {
            case "office1":
                SoundController.Instance.PlayOffice1Death();
                break;
            case "office2":
                SoundController.Instance.PlayOffice2Death();
                break;
            case "office3":
                SoundController.Instance.PlayOffice3Death();
                break;
            case "toilet1":
                SoundController.Instance.PlayToilet1Death();
                break;
            case "toilet2":
                SoundController.Instance.PlayToilet2Death();
                break;
            case "toilet3":
                SoundController.Instance.PlayToilet3Death();
                break;
            case "bossOffice1":
                SoundController.Instance.PlayBossOffice1Death();
                break;
            case "bossOffice2":
                SoundController.Instance.PlayBossOffice2Death();
                break;
            case "bossOffice3":
                SoundController.Instance.PlayBossOffice3Death();
                break;
            default:
                Debug.LogWarning($"No death sound defined for enemy type: {enemyType}");
                break;
        }
    }
}
