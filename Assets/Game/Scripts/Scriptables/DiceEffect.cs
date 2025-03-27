using UnityEngine;
using WS_DiegoCo;
using System.Collections.Generic;
using Unity.VisualScripting;

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

    public override void ApplyEffect(EnemyDisplay enemy, Card cardData, PlayerEvent player, DeckManager deck, HandManager hand, BattleManager battleManager, EnemyManager enemyManager)
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
                    player.GainEnergy(2);
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
                ApplyRandomReward(player, diceRoll, deck, cardData);
                break;

            case DiceType.PerceptionOrHeal:
                if (diceRoll % 2 == 0)
                    player.GainPerception(cardData.perception);
                else
                    player.GainHealth(cardData.health);
                break;
        }
    }

    private void ApplyRandomReward(PlayerEvent player, int diceRoll, DeckManager deck, Card cardData)
    {
        // Example rewards based on the roll
        switch (diceRoll)
        {
            case 1:
                player.GainHealth(cardData.health);
                break;
            case 2:
                player.TakeDamage(cardData.damage);
                break;
            case 3:
                player.GainShield(cardData.defense);
                break;
            case 4:
                player.GainPerception(cardData.perception);
                break;
            case 5:
                player.GainInfiltration(cardData.discretion);
                break;
            case 6:
                player.GainEnergy(3);
                deck.DrawCard(2);
                break;
        }
    }
}