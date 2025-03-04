using UnityEngine;
using WS_DiegoCo;

[CreateAssetMenu(fileName = "New Stat Effect", menuName = "Card Effects/Stat Change")]
public class StatChangeEffect : CardEffect
{
    public Card.StatType affectedStat;
    public int amount;

    public override void ApplyEffect(EnemyDisplay enemy)
    {
        //enemy.ModifyStat(affectedStat, amount);
    }
}