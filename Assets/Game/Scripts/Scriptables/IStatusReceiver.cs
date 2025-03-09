using UnityEngine;

public interface IStatusReceiver
{
    void ApplyStatus(StatusEffect.StatusType statusType, int value, int duration);
    void GainShield(int amount);
}
