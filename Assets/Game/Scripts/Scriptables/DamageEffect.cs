using UnityEngine;
using WS_DiegoCo;

[CreateAssetMenu(fileName = "New Damage Effect", menuName = "Card Effects/Damage")]
public class DamageEffect : CardEffect
{
    // Implementation of the original abstract method (fallback for missing parameters)


    // The new, more flexible method
    public override void ApplyEffect(EnemyDisplay enemy, Card cardData, PlayerEvent player)
    {
        if (enemy == null)
        {
            Debug.LogError("DamageEffect: Enemy is null!");
            return;
        }

        enemy.TakeDamage(cardData.damage);
    }
}