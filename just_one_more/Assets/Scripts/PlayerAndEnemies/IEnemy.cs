using UnityEngine;
// IEnemy.cs
public interface IEnemy
{
    EnemyData GetEnemyData(); // Must return the enemy's runtime data
    Transform GetTransform(); // Must return the enemy's Transform
    void SetEnemyType(string type); // Must set the enemy's type
    string GetEnemyType(); // Must return the enemy's type
    void TakeDamage(int damage); // Must handle taking damage
    void Freeze(float duration); // Must handle freezing the enemy
}
