using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WS_DiegoCo_Enemy;
using System;
using WS_DiegoCo;
using System.Collections.Generic;
using System.Linq;

public class EnemyDisplay : MonoBehaviour, IStatusReceiver
{

    public Enemy enemyData;
    public Sprite enemyImageDisplay;
    public TMP_Text nameText;
    public TMP_Text healthText;
    public TMP_Text defenseText;
    public TMP_Text precisionText;
    public TMP_Text discretionText;
    public TMP_Text damageText;

    private int currentDefense = 0;
    private bool isNextAttackHeal = false;

    private PlayerEvent player;
    private Dictionary<StatusEffect.StatusType, (int value, int turnsRemaining)> activeEffects = new Dictionary<StatusEffect.StatusType, (int, int)>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {  
        
    }

    public void Initialize(Enemy data)
    {
        enemyData = data;
        enemyData.health = enemyData.maxHealth;
        enemyData.damage = enemyData.maxDamage;
        enemyData.discretion = enemyData.maxDiscretion;
        enemyData.perception = enemyData.maxPerception;
        UpdateEnemyDisplay();
    }

    private void UpdateEnemyDisplay()
    {
        nameText.text = enemyData.enemyName;
        healthText.text = enemyData.health.ToString();
        damageText.text = enemyData.damage.ToString();
        enemyImageDisplay = enemyData.enemyImage;
    }

    public void SetNextAttackHeal()
    {
        isNextAttackHeal = true;
        Debug.Log("Next attack will heal the player.");
    }

    public void TakeDamage(int damage, bool ignoreShield = false)
    {
        if (!ignoreShield && currentDefense > 0)
        {
            int absorbed = Mathf.Min(damage, currentDefense);
            currentDefense -= absorbed;
            damage -= absorbed;
            Debug.Log($"Enemy {enemyData.enemyName} shield absorbed {absorbed} damage. Remaining shield: {currentDefense}");
        }

        if (damage > 0)
        {
            enemyData.health -= damage;
            Debug.Log($"Enemy {enemyData.enemyName} took {damage} damage. Remaining health: {enemyData.health}");
        }

        if(isNextAttackHeal == true)
        {
            player.GainHealth(damage);
            isNextAttackHeal = false;
        }

        UpdateEnemyDisplay();
    }

    public void GainShield(int amount)
    {
        currentDefense += amount;
        UpdateEnemyDisplay();
        Debug.Log($"Enemy {enemyData.enemyName} gained {amount} shield. Current shield: {currentDefense}");
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
        enemyData.perception -= value;
        UpdateEnemyDisplay();
    }

    public void ReduceInfiltration(int value)
    {
        enemyData.discretion -= value;
        UpdateEnemyDisplay();
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
        Debug.Log($"Enemy {enemyData.enemyName} applied status {statusType} with value {value} for {duration} turns.");
    }

    public void ProcessTurnEffects()
    {
        List<StatusEffect.StatusType> toRemove = new List<StatusEffect.StatusType>();

        foreach (var key in activeEffects.Keys.ToList())
        {
            var effect = activeEffects[key];

            switch (key)
            {
                case StatusEffect.StatusType.Regeneration:
                    enemyData.health = Mathf.Min(enemyData.health + effect.value, enemyData.maxHealth);
                    Debug.Log($"Enemy {enemyData.enemyName} regenerated {effect.value} HP.");
                    break;

                case StatusEffect.StatusType.Bleeding:
                    TakeDamage(effect.value, ignoreShield: true);
                    Debug.Log($"Enemy {enemyData.enemyName} suffered {effect.value} bleeding damage.");
                    break;

                case StatusEffect.StatusType.Weakness:
                    enemyData.damage -= effect.value;
                    Debug.Log($"Enemy {enemyData.enemyName} is weakened!");
                    break;

                case StatusEffect.StatusType.Strength:
                    enemyData.damage += effect.value;
                    Debug.Log($"Enemy {enemyData.enemyName} strength increased by {effect.value}.");
                    break;
            }

            // Reduce duration
            int newTurns = effect.turnsRemaining - 1;
            if (newTurns <= 0)
            {
                toRemove.Add(key);
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
            Debug.Log($"Enemy {enemyData.enemyName} status {status} expired.");
        }
    }
}
