using UnityEngine;
using WS_DiegoCo;

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
        StrengthFromInfiltration
    }

    public StatusType statusType;
    public int value;
    public int duration;
    public bool applyToPlayer = false;
    public bool applyToEnemy = false;

    public override void ApplyEffect(EnemyDisplay enemy, Card cardData, PlayerEvent player, DeckManager deck, HandManager hand, BattleManager battleManager, EnemyManager enemyManager)
    {
        if (applyToPlayer)
        {
            ApplyStatusEffect(player, statusType, value, duration);
        }

        if (applyToEnemy)
        {
            ApplyStatusEffect(enemy, statusType, value, duration);
        }
    }

    private void ApplyStatusEffect(IStatusReceiver target, StatusType status, int effectValue, int effectDuration)
    {
        switch (status)
        {
            case StatusType.Shield:
                target.ApplyStatus(status, effectValue, effectDuration);
                break;
            case StatusType.Regeneration:
                target.ApplyStatus(status, effectValue, effectDuration);
                break;
            case StatusType.Weakness:
                target.ApplyStatus(status, 1, effectDuration);
                break;
            case StatusType.Strength:
                target.ApplyStatus(status, effectValue, effectDuration);
                break;
            case StatusType.Bleeding:
                target.ApplyStatus(status, effectValue, effectDuration);
                break;
            case StatusType.StrengthFromInfiltration:
                ApplyStrengthFromInfiltration(target);
                break;
        }
    }

    private void ApplyStrengthFromInfiltration(IStatusReceiver target)
    {
        if (target is PlayerEvent player)
        {
            int infiltration = player.cardData.discretion;
            int strengthGain = infiltration / 4;

            player.ApplyStatus(StatusType.Strength, strengthGain, 1); // 1 turn effect
            player.TakeDamage(3);

            Debug.Log($"Player gains {strengthGain} Strength from {infiltration} Infiltration and loses 3 HP.");
        }
    }
}
