using UnityEngine;
using WS_DiegoCo;

[CreateAssetMenu(fileName = "New Damage Effect", menuName = "Card Effects/Damage")]
 public class DamageEffect : CardEffect 
 {
    public enum TargetDamage
    {
        Player,

        Enemy,

        AllEnemies,

        DefenseAttack

    }
    public TargetDamage targetDamage;

    // The new, more flexible method
    public override void ApplyEffect(EnemyDisplay enemy, Card cardData, PlayerEvent player, DeckManager deck, HandManager hand, GameManager gameManager, EnemyManager enemyManager)
    {
        switch (targetDamage)
        {
            case TargetDamage.Player:
                player.TakeDamage(cardData.damage);
                break;

            case TargetDamage.Enemy:
                enemy.TakeDamage(cardData.damage);
                break;

            case TargetDamage.AllEnemies:
                foreach (EnemyDisplay enemyObject in enemyManager.enemies)
                {
                    enemyObject.TakeDamage(cardData.damage);
                }
                break;

            case TargetDamage.DefenseAttack:
                enemy.TakeDamage(player.currentDefense);
                break;
        }      
    }
 }

    
