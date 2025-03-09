using UnityEngine;
using WS_DiegoCo;

[CreateAssetMenu(fileName = "New Gain Effect", menuName = "Card Effects/Stat Gain")]
public class StatChangeEffect : CardEffect
{
   
   public enum StatType
   {
        health,

        defense,

        discretion,

        perception
   }
    public StatType statType;

    public override void ApplyEffect(EnemyDisplay enemy, Card cardData, PlayerEvent player, DeckManager deck, HandManager hand, GameManager gameManager, EnemyManager enemyManager)
    {
        switch (statType)
        {
            case StatType.health:
                player.GainHealth(cardData.health);
                break;

            case StatType.defense:
                player.GainShield(cardData.defense);
                break;

            case StatType.discretion:
                player.GainInfiltration(cardData.discretion);
                break;

            case StatType.perception:
                player.GainPerception(cardData.perception);
                break;
        }
    }
}