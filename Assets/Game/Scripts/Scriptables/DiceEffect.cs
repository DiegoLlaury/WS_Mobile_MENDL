using UnityEngine;
using WS_DiegoCo;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Dice Effect", menuName = "Card Effects/Dice")]
public class DiceEffect : CardEffect
{
    public enum DiceType
    {
        HealOrShieldOrEnergy,
        DamageOrHeal,
        InfiltrationOrAttack,
        TripleDamage,
        RandomReward,
        PerceptionOrHeal
    }

    public DiceType diceType;

    public override void ApplyEffect(EnemyDisplay enemy, Card cardData, PlayerEvent player)
    {
        int diceRoll = Random.Range(1, 7); // Rolling a 6-sided die
        Debug.Log($"Rolled a {diceRoll}");

        switch (diceType)
        {
            case DiceType.HealOrShieldOrEnergy:
                if (diceRoll < 3)
                    player.GainHealth(cardData.health);
                else if (diceRoll < 6)
                    player.GainShield(cardData.defense);
                else
                    player.GainEnergy(1);
                break;

            case DiceType.DamageOrHeal:
                if (diceRoll % 2 == 0)
                    enemy.TakeDamage(diceRoll * 2);
                else
                    player.GainHealth(diceRoll * 2);
                break;

            case DiceType.InfiltrationOrAttack:
                if (diceRoll <= 3)
                    player.GainInfiltration(cardData.discretion);
                else
                    enemy.TakeDamage(diceRoll);
                break;

            case DiceType.TripleDamage:
                enemy.TakeDamage(diceRoll * 3);
                break;

            case DiceType.RandomReward:
                ApplyRandomReward(player, diceRoll);
                break;

            case DiceType.PerceptionOrHeal:
                if (diceRoll % 2 == 0)
                    player.GainPerception(cardData.perception);
                else
                    player.GainHealth(cardData.health);
                break;
        }
    }

    private void ApplyRandomReward(PlayerEvent player, int diceRoll)
    {
        // Example rewards based on the roll
        switch (diceRoll)
        {
            case 1:
                player.GainHealth(5);
                break;
            case 2:
                player.GainEnergy(2);
                break;
            case 3:
                player.GainPerception(3);
                break;
            case 4:
                player.GainShield(4);
                break;
            case 5:
                player.GainInfiltration(2);
                break;
            case 6:
                player.GainHealth(10);
                break;
        }
    }
}