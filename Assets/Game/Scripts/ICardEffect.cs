using UnityEngine;
using WS_DiegoCo;

public interface ICardEffect
{
    public void ApplyEffect(EnemyDisplay enemy, Card cardData, PlayerEvent player); // Apply effect on an enemy
}
