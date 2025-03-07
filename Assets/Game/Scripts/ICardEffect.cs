using UnityEngine;
using WS_DiegoCo;

public interface ICardEffect
{
    public void ApplyEffect(EnemyDisplay enemy, Card cardData, PlayerEvent player, DeckManager deck, HandManager hand, GameManager gameManager, EnemyManager enemyManager); // Apply effect on an enemy
}
