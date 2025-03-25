using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WS_DiegoCo_Enemy;
using System;
using WS_DiegoCo;
using System.Collections.Generic;
using System.Linq;
using WS_DiegoCo_Event;
using UnityEngine.InputSystem;

public class EnemyDisplay : MonoBehaviour, IStatusReceiver
{
    private EventBattle.EventType currentEventType;
    public Enemy enemyData;


    public Sprite enemyImageDisplay;
    public TMP_Text nameText;
    public TMP_Text healthText;
    public TMP_Text defenseText;
    public TMP_Text perceptionText;
    public TMP_Text discretionText;
    public TMP_Text damageText;


    public GameObject combatStatPanel;
    public GameObject infiltrationStatPanel;
    public GameObject investigationStatPanel;

    private bool isNextAttackHeal;

    private PlayerEvent player;
    private Dictionary<StatusEffect.StatusType, (int value, int turnsRemaining)> activeEffects = new Dictionary<StatusEffect.StatusType, (int, int)>();

    public void Initialize(Enemy data, EventBattle.EventType eventType)
    {
        enemyData = data;
        currentEventType = eventType;
        player = FindAnyObjectByType<PlayerEvent>();

        enemyData.health = enemyData.maxHealth;
        enemyData.damage = enemyData.maxDamage;
        enemyData.discretion = enemyData.maxDiscretion;
        enemyData.perception = enemyData.maxPerception;
        enemyData.defense = enemyData.maxDefense;
        isNextAttackHeal = false;
        UpdateEnemyDisplay();
    }

    public void UpdateEnemyDisplay()
    {
        nameText.text = enemyData.enemyName;
        healthText.text = enemyData.health.ToString();
        defenseText.text = enemyData.defense.ToString();
        damageText.text = enemyData.damage.ToString();
        perceptionText.text = enemyData.perception.ToString();
        discretionText.text = enemyData.discretion.ToString();
        enemyImageDisplay = enemyData.enemyImage;

        combatStatPanel.SetActive(currentEventType == EventBattle.EventType.Combat);
        infiltrationStatPanel.SetActive(currentEventType == EventBattle.EventType.Infiltration);
        investigationStatPanel.SetActive(currentEventType == EventBattle.EventType.Enquete);
    }

    public void SetNextAttackHeal()
    {
        isNextAttackHeal = true;
        Debug.Log("Next attack will heal the player.");
    }

    public void TakeDamage(int damage, bool ignoreShield = false)
    {
        Debug.Log(isNextAttackHeal);
        if (!ignoreShield && enemyData.defense > 0)
        {
            int absorbed = Mathf.Min(damage, enemyData.defense);
            enemyData.defense -= absorbed;
            damage -= absorbed;
            Debug.Log($"Enemy {enemyData.enemyName} shield absorbed {absorbed} damage. Remaining shield: {enemyData.defense}");
        }

        if (damage > 0)
        {
            enemyData.health -= damage;
            Debug.Log($"Enemy {enemyData.enemyName} took {damage} damage. Remaining health: {enemyData.health}");
        }

        if (isNextAttackHeal == true)
        {
            player.GainHealth(damage);
            isNextAttackHeal = false;
        }

        if(GameManager.currentEvent.eventType == EventBattle.EventType.Infiltration)
        {
            GameManager.currentEvent.currentTurn = 2;
        }
        
        if(GameManager.currentEvent.eventType == EventBattle.EventType.Enquete)
        {
            Debug.Log("You failed !");
        }

        UpdateEnemyDisplay();
        player.Attack();
    }

    public void GainShield(int amount)
    {
        enemyData.defense += amount;
        UpdateEnemyDisplay();
        Debug.Log($"Enemy {enemyData.enemyName} gained {amount} shield. Current shield: {enemyData.defense}");
    }

    public void ModifyStat(Card.StatType stat, int amount)
    {
        switch (stat)
        {
            case Card.StatType.health:
                enemyData.health += amount;
                break;
            case Card.StatType.defense:
                enemyData.defense += amount;
                break;
            case Card.StatType.damage:
                enemyData.damage += amount;
                break;
            case Card.StatType.discretion:
                enemyData.discretion += amount;
                break;
            case Card.StatType.perception:
                enemyData.perception += amount;
                break;
        }
        UpdateEnemyDisplay();
        Debug.Log($"Enemy {stat} changed by {amount}");
    }

    public void ReducePerception(int value)
    {
        enemyData.perception = Mathf.Clamp(enemyData.perception - value, enemyData.maxPerception, 50);
        UpdateEnemyDisplay();
    }

    public void ReduceInfiltration(int value)
    {
        enemyData.discretion = Mathf.Clamp(enemyData.discretion - value, enemyData.maxDiscretion, 50);
        UpdateEnemyDisplay();
    }

    public void ApplyStatus(StatusEffect.StatusType statusType, int value, int duration, EnemyDisplay enemy)
    {
        if (activeEffects.ContainsKey(statusType))
        {
            enemyData = enemy.enemyData;
            // Refresh duration or stack values
            activeEffects[statusType] = (activeEffects[statusType].value + value, duration);

            
        }
        else
        {
            activeEffects.Add(statusType, (value, duration));
        }

        switch (statusType)
        {
            case StatusEffect.StatusType.Weakness:
                enemyData.damage -= value;
                Debug.Log($"Enemy {enemyData.enemyName} is weakened!");
                break;

            case StatusEffect.StatusType.Strength:
                enemyData.damage += value;
                Debug.Log($"Enemy {enemyData.enemyName} strength increased by {value}.");
                break;
        }

        UpdateEnemyDisplay();
        Debug.Log($"Enemy {enemyData.enemyName} applied status {statusType} with value {value} for {duration} turns.");
    }

    public void ProcessTurnEffects(bool endStatus)
    {
        List<StatusEffect.StatusType> toRemove = new List<StatusEffect.StatusType>();

        foreach (var key in activeEffects.Keys.ToList())
        {
            var effect = activeEffects[key];

            if (!endStatus)
            {
                switch (key)
                {
                    case StatusEffect.StatusType.Regeneration:
                        enemyData.health = Mathf.Clamp(enemyData.health + effect.value, 0, enemyData.maxHealth);
                        Debug.Log($"Enemy {enemyData.enemyName} regenerated {effect.value} HP.");
                        break;

                    case StatusEffect.StatusType.Bleeding:
                        TakeDamage(effect.value, ignoreShield: true);
                        Debug.Log($"Enemy {enemyData.enemyName} suffered {effect.value} bleeding damage.");
                        break;
                }
            }


            if (endStatus)
            {
                // Reduce duration
                int newTurns = effect.turnsRemaining - 1;
                if (newTurns <= 0)
                {
                    toRemove.Add(key);

                    if (endStatus)
                    {
                        switch (key)
                        {
                            case StatusEffect.StatusType.Weakness:
                                enemyData.damage += effect.value;
                                Debug.Log($"Enemy {enemyData.enemyName} is weakened!");
                                break;

                            case StatusEffect.StatusType.Strength:
                                enemyData.damage -= effect.value;
                                Debug.Log($"Enemy {enemyData.enemyName} strength increased by {effect.value}.");
                                break;
                        }
                    }
                }
                else
                {
                    activeEffects[key] = (effect.value, newTurns);
                }
            }

            UpdateEnemyDisplay();
        }

        // Remove expired effects
        foreach (var status in toRemove)
        {
            activeEffects.Remove(status);
            Debug.Log($"Enemy {enemyData.enemyName} status {status} expired.");
        }
    }
}
