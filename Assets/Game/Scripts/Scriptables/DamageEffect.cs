using UnityEngine;
using WS_DiegoCo;

[CreateAssetMenu(fileName = "New Damage Effect", menuName = "Card Effects/Damage")]
public class DamageEffect : CardEffect
{
    private Card carddata;

    public override void ApplyEffect(EnemyDisplay enemy)
    {
        enemy.TakeDamage(carddata.damage);
    }
}