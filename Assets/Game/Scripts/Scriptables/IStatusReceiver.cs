using UnityEngine;

public interface IStatusReceiver
{
    void ApplyStatus(StatusEffect.StatusType statusType, int value, int duration, EnemyDisplay enemy);
    void GainShield(int amount);
}
