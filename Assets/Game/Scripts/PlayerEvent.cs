using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using WS_DiegoCo_Middle;
using WS_DiegoCo;
using UnityEngine.Rendering;
using System.Linq;
using WS_DiegoCo_Enemy;

public class PlayerEvent : MonoBehaviour, IStatusReceiver
{

    public CardMiddle cardData;
    public GameManager game;
    private Card card;
    private EnemyDisplay enemy;

    public TMP_Text heartText;
    public TMP_Text spadeText;
    public TMP_Text squareText;
    public TMP_Text cloverText;
    public TMP_Text energyText;
    public TMP_Text defenseText;
    public TMP_Text healthText;
    public TMP_Text strenghtText;
    public TMP_Text discretionText;
    public TMP_Text perceptionText;
    public TMP_Text currentTurnText;

    [SerializeField] private int healthRatio = 5;

    public int maxEnergy = 3;
    private int currentEnergy;
    public int currentDefense;
    private Dictionary<StatusEffect.StatusType, (int value, int turnsRemaining)> activeEffects = new Dictionary<StatusEffect.StatusType, (int, int)>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentEnergy = maxEnergy;
        cardData.maxHealth = cardData.heart * healthRatio;
        cardData.health = cardData.maxHealth;
        cardData.strenght = cardData.maxStrenght;
        cardData.discretion = cardData.maxDiscretion;
        cardData.perception = cardData.maxPerception;
        cardData.defense = currentDefense;
        game.currentEvent.currentTurn = game.currentEvent.numberTurn;
        UpdatePlayerEvent();
    }

    private void UpdatePlayerEvent()
    {
        healthText.text = cardData.health.ToString();
        strenghtText.text = cardData.strenght.ToString();
        discretionText.text = cardData.discretion.ToString();
        perceptionText.text = cardData.perception.ToString();
        energyText.text = currentEnergy.ToString();
        defenseText.text = currentDefense.ToString();
        currentTurnText.text = game.currentEvent.currentTurn.ToString();
    }

    public void TurnChange()
    {
        game.currentEvent.currentTurn--;
        UpdatePlayerEvent();
    }

    public void TakeDamage(int damage, bool ignoreShield = false)
    {
        if (!ignoreShield && currentDefense > 0)
        {
            int absorbed = Mathf.Min(damage, currentDefense);
            currentDefense -= absorbed;
            damage -= absorbed;
            Debug.Log($"Shield absorbed {absorbed} damage. Remaining shield: {currentDefense}");
        }

        if (damage > 0)
        {
            cardData.health -= damage;
            Debug.Log($"Player took {damage} damage. Remaining health: {cardData.health}");
        }

        UpdatePlayerEvent();

        if (cardData.health <= 0)
        {
            BattleManager.Instance.CheckGameOver();
        }
    }

    public void GainHealth(int health)
    {
        if (cardData.health < cardData.maxHealth)
        {
            cardData.health = Mathf.Min(cardData.health + health, cardData.maxHealth);
            Debug.Log($"Player gained {health} health. Current health: {cardData.health}");
            UpdatePlayerEvent();
        }
        else
        {
            Debug.Log("Health is already at max.");
        }
    }

    public void GainShield(int defense)
    {
        currentDefense += defense;
        UpdatePlayerEvent();
        Debug.Log($"Player gain {defense} shield. Current shield : {cardData.defense}");
    }

    public void GainPerception(int perception)
    {
        cardData.perception += perception;
        UpdatePlayerEvent();
        Debug.Log($"Player gain {perception} perception. Current perception : {cardData.perception}");
    }

    public void GainInfiltration(int infiltration)
    {
        cardData.discretion += infiltration;
        UpdatePlayerEvent();
        BattleManager.Instance.CheckGameOver();
        Debug.Log($"Player gain {infiltration} discretion. Current discretion : {cardData.discretion}");
    }

    public void GainEnergy(int energy)
    {
        currentEnergy += energy;
        UpdatePlayerEvent();
        Debug.Log($"Player gain {energy} energy. Current Energy : {currentEnergy}");
    }

    public void ApplyDebuff(Card.StatType stat, int amount)
    {
           switch (stat)
           {
            case Card.StatType.health:
                cardData.health += amount;
                break;
            case Card.StatType.damage:
                cardData.strenght += amount;
                break;
            case Card.StatType.discretion:
                cardData.discretion += amount;
                break;
            case Card.StatType.perception:
                cardData.perception += amount;
                break;
           }

           UpdatePlayerEvent();
           Debug.Log($"Player {stat} changed by {amount}");
    }

    public void ApplyStatus(StatusEffect.StatusType statusType, int value, int duration)
    {
        if (activeEffects.ContainsKey(statusType))
        {
            // Refresh duration or stack values
            activeEffects[statusType] = (activeEffects[statusType].value + value, duration);
        }
        else
        {
            activeEffects.Add(statusType, (value, duration));
        }

        if (statusType == StatusEffect.StatusType.Strength)
        {
            cardData.strenght += value;
        }
        else if (statusType == StatusEffect.StatusType.Weakness)
        {
            cardData.strenght -= value;
        }

        UpdatePlayerEvent();
        CardDisplay.UpdateAllCards(cardData.strenght);
        Debug.Log($"Applied status {statusType} with value {value} for {duration} turns.");
    }

    public void ProcessTurnEffects()
    {
        List<StatusEffect.StatusType> toRemove = new List<StatusEffect.StatusType>();

        foreach (var key in activeEffects.Keys.ToList())
        {
            var effect = activeEffects[key];

            switch (key)
            {
                case StatusEffect.StatusType.Shield:
                    GainShield(effect.value);
                    break;
                case StatusEffect.StatusType.Regeneration:
                    GainHealth(effect.value);
                    break;
                case StatusEffect.StatusType.Bleeding:
                    TakeDamage(effect.value, ignoreShield: true);
                    break;
                case StatusEffect.StatusType.Weakness:
                    cardData.strenght -= effect.value;
                    break;
                case StatusEffect.StatusType.Strength:
                    cardData.strenght += effect.value;
                    Debug.Log($"Player strength increased by {effect.value}");
                    break;
            }

            // Reduce duration
            int newTurns = effect.turnsRemaining - 1;
            if (newTurns <= 0)
            {
                toRemove.Add(key);

                // Remove effect from Strength/Weakness
                if (key == StatusEffect.StatusType.Strength)
                {
                    cardData.strenght -= effect.value;
                }
                else if (key == StatusEffect.StatusType.Weakness)
                {
                    cardData.strenght += effect.value;
                }
            }
            else
            {
                activeEffects[key] = (effect.value, newTurns);
            }
        }
        // Remove expired effects
        foreach (var status in toRemove)
        {
            activeEffects.Remove(status);
            Debug.Log($"Status {status} expired.");
        }

        UpdatePlayerEvent();
        CardDisplay.UpdateAllCards(cardData.strenght);
    }

    public void RondeTest(EnemyDisplay enemy)
    {
        if (cardData.discretion < enemy.enemyData.perception)
        {
            Debug.Log("Infiltration Failed!");
        }
    }

    public bool CanPlayCard(int cost)
    {
        return currentEnergy >= cost;
    }

    public void UseEnergy( int cost)
    {
        if (currentEnergy >= cost)
        {
            currentEnergy -= cost;
            UpdatePlayerEvent();
        }
        else
        {
            Debug.LogWarning("Not enough energy to play this card!");
        }
    }

    public void ResetEnergy()
    {
        currentEnergy = maxEnergy;
        UpdatePlayerEvent();
    }  
}

