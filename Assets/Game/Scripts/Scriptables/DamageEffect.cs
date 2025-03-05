using UnityEngine;
using WS_DiegoCo;

[CreateAssetMenu(fileName = "New Damage Effect", menuName = "Card Effects/Damage")]
public class DamageEffect : CardEffect
{
    // Implementation of the original abstract method (fallback for missing parameters)
    public override void ApplyEffect(EnemyDisplay enemy)
    {
        ApplyEffect(enemy, 0); // Default to 0 damage if no value is given
    }

    // The new, more flexible method
    public override void ApplyEffect(EnemyDisplay enemy, int damage)
    {
        if (enemy == null)
        {
            Debug.LogError("DamageEffect: Enemy is null!");
            return;
        }

        enemy.TakeDamage(damage);
    }
}