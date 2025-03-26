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

        perception,

        GainPerceptionButLostDiscretion,

        ReduceEnemyInfiltration,

        ReduceEnemyPerception,

        GainHealthIfDiscrection,

        GainDiscretionButLostPerception
    }
    public StatType statType;

    public override void ApplyEffect(EnemyDisplay enemy, Card cardData, PlayerEvent player, DeckManager deck, HandManager hand, BattleManager battleManager, EnemyManager enemyManager)
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

            case StatType.GainPerceptionButLostDiscretion:
                if (player.cardData.discretion == 0)
                {
                 
                }
                else
                {
                    player.GainPerception(player.cardData.discretion);
                    player.cardData.discretion = 0;
                }

                break;

            case StatType.ReduceEnemyInfiltration:
                int reductionAmount = player.cardData.perception / 2;
                enemy.ReduceInfiltration(reductionAmount);
                break;

            case StatType.ReduceEnemyPerception:
                enemy.ReducePerception(cardData.perception);
                break;

            case StatType.GainHealthIfDiscrection:
                Debug.Log("Hallo y a quelqu'un ???");
                if (player.cardData.discretion >= 10)
                {
                    shouldReturnToHand = true;
                    Debug.Log("Pk");
                }
                else
                {
                    player.GainHealth(cardData.health);
                }
                break;

            case StatType.GainDiscretionButLostPerception:
                if (player.cardData.perception == 0)
                {
                   
                }
                else
                {
                    player.GainInfiltration(player.cardData.perception);
                    player.cardData.perception = 0;
                } 
                break;
        }
    }
}