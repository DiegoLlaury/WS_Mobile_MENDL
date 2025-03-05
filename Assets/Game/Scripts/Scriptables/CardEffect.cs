using UnityEngine;

public abstract class CardEffect : ScriptableObject, ICardEffect
{
    
    public abstract void ApplyEffect(EnemyDisplay enemy);

    public virtual void ApplyEffect(EnemyDisplay enemy, int value)
    {
        ApplyEffect(enemy);
    }
}
