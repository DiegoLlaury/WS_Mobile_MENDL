using UnityEngine;
using WS_DiegoCo;

[CreateAssetMenu(fileName = "New OddEven Effect", menuName = "Card Effects/Odd Even Effect")]
public class OddEvenEffect : CardEffect
{
    public enum StatType
    {
        health,
        discretion,  // Infiltration
        perception
    }

    public StatType statType;

    public override void ApplyEffect(EnemyDisplay enemy, Card cardData, PlayerEvent player, DeckManager deck, HandManager hand, BattleManager battleManager, EnemyManager enemyManager)
    {
        switch (statType)
        {
            case StatType.health:
                if (player.cardData.health % 2 == 0)
                {
                    player.GainShield(cardData.defense);
                    Debug.Log("HP is even: Gained " + cardData.defense + " shield.");
                }
                else
                {
                    player.GainHealth(cardData.health);
                    Debug.Log("HP is odd: Gained " + cardData.health + " HP.");
                }
                break;

            case StatType.discretion: // Infiltration
                if (player.cardData.discretion % 2 == 0)
                {
                    player.GainInfiltration(cardData.discretion);
                    Debug.Log("Infiltration is even: Gained " + cardData.discretion + " infiltration.");
                }
                else
                {
                    enemy.TakeDamage(cardData.damage);
                    Debug.Log("Infiltration is odd: Dealt " + cardData.damage + " damage to enemy.");
                }
                break;

            case StatType.perception:
                if (player.cardData.perception % 2 == 0)
                {
                    player.GainPerception(cardData.perception);
                    Debug.Log("Perception is even: Gained " + cardData.perception + " perception.");
                }
                else
                {
                    player.GainInfiltration(cardData.discretion);
                    Debug.Log("Perception is odd: Gained " + cardData.discretion + " infiltration.");
                }
                break;
        }
    }
}
