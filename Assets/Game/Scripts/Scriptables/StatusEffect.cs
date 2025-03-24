using UnityEngine;
using WS_DiegoCo;
using WS_DiegoCo_Enemy;

[CreateAssetMenu(fileName = "New Status Effect", menuName = "Card Effects/Status")]
public class StatusEffect : CardEffect
{
    public enum StatusType
    {
        Shield,
        Regeneration,
        Weakness,
        Strength,
        Bleeding,
        StrengthFromInfiltration,
        StrenghtNextTurn
    }

    public StatusType statusType;
    public int value;
    public int duration;
    public bool applyToPlayer = false;
    public bool applyToEnemy = false;
    public int NumberOfdamage;
    private EnemyDisplay enemyChoose;

    public override void ApplyEffect(EnemyDisplay enemy, Card cardData, PlayerEvent player, DeckManager deck, HandManager hand, BattleManager battleManager, EnemyManager enemyManager)
    {
        if (applyToPlayer)
        {
            ApplyStatusEffect(player, statusType, value, duration);
        }

        if (applyToEnemy)
        {
            if (enemy != null)
            {
                enemyChoose = enemy;
                ApplyStatusEffect(enemy, statusType, value, duration);
            }
            else
            {
                Debug.LogWarning("No enemy found to apply the status effect.");
            }
        }
    }

    private void ApplyStatusEffect(IStatusReceiver target, StatusType status, int effectValue, int effectDuration)
    {
        switch (status)
        {
            case StatusType.Shield:
                target.ApplyStatus(status, effectValue, effectDuration, enemyChoose);
                break;
            case StatusType.Regeneration:
                target.ApplyStatus(status, effectValue, effectDuration, enemyChoose);
                break;
            case StatusType.Weakness:
                target.ApplyStatus(status, 1, effectDuration, enemyChoose);
                break;
            case StatusType.Strength:
                target.ApplyStatus(status, effectValue, effectDuration, enemyChoose);
                break;
            case StatusType.Bleeding:
                target.ApplyStatus(status, effectValue, effectDuration, enemyChoose);
                break;
            case StatusType.StrengthFromInfiltration:
                ApplyStrengthFromInfiltration(target);
                break;
            case StatusType.StrenghtNextTurn:
                ApplyStrengthNextTurn(target, effectValue);
                break;
        }
    }

    private void ApplyStrengthFromInfiltration(IStatusReceiver target)
    {
        if (target is PlayerEvent player)
        {
            int infiltration = player.cardData.discretion;
            int strengthGain = infiltration / 4;

            player.ApplyStatus(StatusType.Strength, strengthGain, 1, enemyChoose); // 1 turn effect
            
            player.cardData.health = Mathf.Clamp(player.cardData.health + NumberOfdamage, 0, player.cardData.maxHealth);

            Debug.Log($"Player gains {strengthGain} Strength from {infiltration} Infiltration and loses 3 HP.");
        }
    }

    private void ApplyStrengthNextTurn(IStatusReceiver target, int strengthGain)
    {
        if (target is PlayerEvent player)
        {
            // Ajoute un effet différé pour le prochain tour
            player.AddTemporaryEffect(() =>
            {
                player.ApplyStatus(StatusType.Strength, strengthGain, 1, enemyChoose);
                Debug.Log($"Player gains {strengthGain} Strength for this turn.");
            });

            Debug.Log($"Player will gain {strengthGain} Strength next turn.");
        }
    }
}
