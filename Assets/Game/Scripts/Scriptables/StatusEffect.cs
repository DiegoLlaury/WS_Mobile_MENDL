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
        Bleeding
    }

    public StatusType statusType;
    public int value;
    public int duration;
    public bool applyToPlayer = false;
    public bool applyToEnemy = false;

    public override void ApplyEffect(EnemyDisplay enemy, Card cardData, PlayerEvent player, DeckManager deck, HandManager hand, GameManager gameManager, EnemyManager enemyManager)
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
                target.GainShield(effectValue);
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
        }
    }
}
