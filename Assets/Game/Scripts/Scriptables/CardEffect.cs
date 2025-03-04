using UnityEngine;

public abstract class CardEffect : ScriptableObject, ICardEffect
{
    public abstract void ApplyEffect(EnemyDisplay enemy);
}
