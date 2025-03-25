using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using WS_DiegoCo_Middle;
using WS_DiegoCo;
using UnityEngine.Rendering;
using System.Linq;
using WS_DiegoCo_Enemy;

public class PlayerEvent : MonoBehaviour, IStatusReceiver
{

    public CardMiddle cardData;
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

    public GameObject widgetLost;
    public GameObject widgetWin;

    public Image backgroundImage;

    [SerializeField] private int healthRatio = 5;

    private bool discretionboost = false;
    public int maxEnergy = 3;
    [SerializeField] private int baseHealth = 10;
    private int currentEnergy;
    public int currentDefense;
    private Dictionary<StatusEffect.StatusType, (int value, int turnsRemaining)> activeEffects = new Dictionary<StatusEffect.StatusType, (int, int)>();
    private List<System.Action> temporaryEffects = new List<System.Action>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cardData = GameManager.selectedCard;
        currentEnergy = maxEnergy;

        // Recalcule les PV max proprement sans empilement
        cardData.maxHealth = baseHealth + cardData.heart * healthRatio;
        cardData.health = cardData.maxHealth;

        // Remet les autres stats
        cardData.strenght = cardData.maxStrenght;
        cardData.discretion = cardData.maxDiscretion;
        cardData.perception = cardData.maxPerception;
        cardData.defense = currentDefense;

        // Met à jour l'interface
        backgroundImage.sprite = GameManager.currentEvent.background;
        GameManager.currentEvent.currentTurn = GameManager.currentEvent.numberTurn;
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
        currentTurnText.text = GameManager.currentEvent.currentTurn.ToString();
        
    }

    public void TurnChange()
    {
        foreach (var effect in temporaryEffects)
        {
            effect.Invoke();
        }
        temporaryEffects.Clear();  // Vide la liste après l'application

        GameManager.currentEvent.currentTurn--;
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
            cardData.health  -= damage;
            Debug.Log($"Player took {damage} damage. Remaining health: {cardData.health}");
        }

        UpdatePlayerEvent();

        if (cardData.health <= 0)
        {
            BattleManager.Instance.CheckGameOver();
        }
    }

    public void Attack()
    {
        // Lors d'une attaque, perdre toute la discrétion
        if (cardData.discretion > 0)
        {
            cardData.strenght -= cardData.discretion / 3;
            cardData.discretion = 0;
            discretionboost = false;
            Debug.Log("Attaque effectuée : Discrétion remise à zéro.");
            UpdatePlayerEvent();
        }
    }

    public void GainHealth(int health)
    {
        cardData.health = Mathf.Clamp(cardData.health + health, 0, cardData.maxHealth);
        Debug.Log($"Player gain {health} health. Current health : {cardData.health}");
    }

    public void GainShield(int defense)
    {
        currentDefense += defense;
        UpdatePlayerEvent();
        Debug.Log($"Player gain {defense} shield. Current shield : {cardData.defense}");
    }

    public void GainPerception(int perception)
    {
        // Ajout normal de perception, en respectant la limite max
        cardData.perception = Mathf.Clamp(cardData.perception + perception, cardData.maxPerception, 50);
        UpdatePlayerEvent();
        Debug.Log($"Player gained {perception} perception. Current perception: {cardData.perception}");
    }

    public void GainInfiltration(int infiltration)
    {
        // Ajout normal de discrétion, en respectant la limite max
        cardData.discretion = Mathf.Clamp(cardData.discretion + infiltration, cardData.maxDiscretion, 50);
        UpdatePlayerEvent();
        Debug.Log($"Player gained {infiltration} discretion. Current discretion: {cardData.discretion}");

        // Boost temporaire de Force si la discrétion dépasse 10 et que le boost n'a pas encore été appliqué
        if (cardData.discretion > 10 && !discretionboost)
        {
            cardData.strenght += cardData.discretion / 3;
            Debug.Log("Discretion > 10: Temporary Strength boost applied.");
            discretionboost = true;
        }
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
            case Card.StatType.damage:
                cardData.strenght += amount;
                break;
            case Card.StatType.discretion:
                GainInfiltration(amount);
                break;
            case Card.StatType.perception:
                GainPerception(amount);
                break;
           }

           UpdatePlayerEvent();
           Debug.Log($"Player {stat} changed by {amount}");
    }

    public void ApplyStatus(StatusEffect.StatusType statusType, int value, int duration, EnemyDisplay enemy)
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

    public void AddTemporaryEffect(System.Action effect)
    {
        temporaryEffects.Add(effect);
        Debug.Log("Temporary effect added for next turn.");
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

    public void EndBattle()
    {
        if (GameManager.WinBattle == true)
        {
            widgetWin.SetActive(true);
        }
        else
        {
            widgetLost.SetActive(true);
        }
    }

    public void DebugBattleWin()
    {
        GameManager.WinBattle = true;
        EndBattle();
    }

    public void DebugBattleLost()
    {
        GameManager.WinBattle = false;
        EndBattle();
    }

    public void ReturnToMain()
    {
        Debug.Log("Clique");
        GameManager.EndEvent();
    }
}