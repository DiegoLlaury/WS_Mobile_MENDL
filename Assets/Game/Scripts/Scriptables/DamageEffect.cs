using UnityEngine;
using WS_DiegoCo;

[CreateAssetMenu(fileName = "New Damage Effect", menuName = "Card Effects/Damage")]
 public class DamageEffect : CardEffect 
 {
    public enum TargetDamage
    {
        Player,

        Enemy
    }
    public TargetDamage targetDamage;

    // The new, more flexible method
    public override void ApplyEffect(EnemyDisplay enemy, Card cardData, PlayerEvent player, DeckManager deck, HandManager hand)
    {
        switch (targetDamage)
        {
            case TargetDamage.Player:
                player.TakeDamage(cardData.damage);
                break;

            case TargetDamage.Enemy:
                enemy.TakeDamage(cardData.damage);
                break;

        }      
    }
 }

    
