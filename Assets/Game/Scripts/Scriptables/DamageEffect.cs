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

        DoubleAttack,

        DefenseAttack,

        InfiltrationAttack,

        RandomEnemy,

        RegenerateNextAttack,   // NEW: Heal HP equal to next attack
        
        SplashDamage,           // NEW: Deal full damage to target, half to others
        
        MultiTargetDamage,      // NEW: Deal damage to two enemies
       
        GambleDamage            // NEW: 50% chance to deal damage or take damage
    }
    public TargetDamage targetDamage;

    // The new, more flexible method
    public override void ApplyEffect(EnemyDisplay enemy, Card cardData, PlayerEvent player, DeckManager deck, HandManager hand, BattleManager battleManager, EnemyManager enemyManager)
    {
        switch (targetDamage)
        {
            case TargetDamage.Player:
                player.TakeDamage(cardData.damage, ignoreShield: false);
                break;

            case TargetDamage.Enemy:
                enemy.TakeDamage(cardData.damage, ignoreShield: false);
                break;

            case TargetDamage.AllEnemies:
                foreach (EnemyDisplay enemyObject in enemyManager.enemies)
                {
                    enemyObject.TakeDamage(cardData.damage, ignoreShield: false);
                }
                break;

            case TargetDamage.DoubleAttack:
                enemy.TakeDamage(cardData.damage * 2, ignoreShield: false);
                break;

            case TargetDamage.DefenseAttack:
                enemy.TakeDamage(player.currentDefense, ignoreShield: false);
                break;

            case TargetDamage.InfiltrationAttack:
                enemy.TakeDamage(player.cardData.discretion * 2, ignoreShield: false);
                break;

            case TargetDamage.RandomEnemy:
                for (int i = 0; i == 5; i++)
                {
                    EnemyDisplay randomEnemy = enemyManager.GetRandomEnemy();
                    if (randomEnemy != null)
                    {
                        randomEnemy.TakeDamage(cardData.damage, ignoreShield: false);
                        Debug.Log($"Random enemy {randomEnemy.enemyData.enemyName} took {cardData.damage} damage.");
                    }
                }
                break;

            case TargetDamage.RegenerateNextAttack:
                enemy.SetNextAttackHeal(); // Method to heal after next attack
                break;

            case TargetDamage.SplashDamage:
                enemy.TakeDamage(cardData.damage);
                foreach (EnemyDisplay otherEnemy in enemyManager.enemies)
                {
                    if (otherEnemy != enemy)
                    {
                        otherEnemy.TakeDamage(cardData.damage / 2, ignoreShield: false);
                    }
                }
                Debug.Log($"Dealt {cardData.damage} to main target, {cardData.damage / 2} to others.");
                break;

            case TargetDamage.MultiTargetDamage:
                if (enemyManager.enemies.Count > 1)
                {
                    enemyManager.enemies[0].TakeDamage(cardData.damage, ignoreShield: false);
                    enemyManager.enemies[1].TakeDamage(cardData.damage, ignoreShield: false);
                    Debug.Log($"Dealt {cardData.damage} damage to two enemies.");
                }
                else if (enemyManager.enemies.Count == 1)
                {
                    enemyManager.enemies[0].TakeDamage(cardData.damage * 2, ignoreShield: false);
                    Debug.Log($"Only one enemy, dealt {cardData.damage} damage.");
                }
                break;

            case TargetDamage.GambleDamage:
                if (Random.value < 0.5f) // 50% chance
                {
                    enemy.TakeDamage(cardData.damage, ignoreShield: false);
                    Debug.Log($"Lucky! Dealt {cardData.damage} damage.");
                }
                else
                {
                    player.TakeDamage(cardData.damage, ignoreShield: false);
                    Debug.Log("Unlucky! Took 5 damage instead.");
                }
                break;
        }      
    }
 }

    
