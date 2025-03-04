using UnityEngine;

[CreateAssetMenu(fileName = "New Damage Effect", menuName = "Card Effects/Damage")]
public class DamageEffect : CardEffect
{
    public int damageAmount;

    public override void ApplyEffect(EnemyDisplay enemy)
    {
        enemy.TakeDamage(damageAmount);
    }
}