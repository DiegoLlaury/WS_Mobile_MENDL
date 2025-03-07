using UnityEngine;
using WS_DiegoCo;

[CreateAssetMenu(fileName = "New Stat Effect", menuName = "Card Effects/Stat Change")]
public class StatChangeEffect : CardEffect
{
   public Card.StatType affectedStat;
    public int amount;


    public override void ApplyEffect(EnemyDisplay enemy, Card cardData, PlayerEvent player, DeckManager deck, HandManager hand)
    {
      enemy.ModifyStat(affectedStat, amount);
    }
}