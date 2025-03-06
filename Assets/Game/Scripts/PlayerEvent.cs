using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using WS_DiegoCo_Middle;
using WS_DiegoCo;
using UnityEngine.Rendering;

public class PlayerEvent : MonoBehaviour
{

    public CardMiddle cardData;
    private Card card;

    public TMP_Text heartText;
    public TMP_Text spadeText;
    public TMP_Text squareText;
    public TMP_Text cloverText;
    public TMP_Text energyText;
    
    public int maxEnergy = 3;
    private int currentEnergy;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentEnergy = maxEnergy;
        cardData.health = cardData.maxHealth;
        cardData.damage = cardData.maxDamage;
        cardData.discretion = cardData.maxDiscretion;
        cardData.perception = cardData.maxPerception;
        UpdatePlayerEvent();
    }

    private void UpdatePlayerEvent()
    {
        heartText.text = cardData.health.ToString();
        spadeText.text = cardData.damage.ToString();
        squareText.text = cardData.discretion.ToString();
        cloverText.text = cardData.perception.ToString();
        energyText.text = currentEnergy.ToString();
       
    }

    public void TakeDamage(int damage)
    {
        cardData.health -= damage;
        UpdatePlayerEvent();
        Debug.Log($"Player took {damage} damage. Remaining health: {cardData.health}");

        if (cardData.health <= 0)
        {
            GameManager.Instance.CheckGameOver();
        }
    }

    public void GainHealth(int health)
    {
        cardData.health = Mathf.Clamp(cardData.health, 0, cardData.maxHealth);
        if (cardData.health < cardData.maxHealth)
        {
            cardData.health += health;
            UpdatePlayerEvent();
        }
        else
        {

        }
    }

    public void GainShield(int shield)
    {
        
    }

    public void GainPerception(int perception)
    {
        cardData.perception += perception;
        UpdatePlayerEvent();
    }

    public void GainInfiltration(int infiltration)
    {
        cardData.discretion += infiltration;
        UpdatePlayerEvent();
    }

    public void GainEnergy(int energy)
    {
        currentEnergy += energy;
        UpdatePlayerEvent();
    }

    public void ApplyDebuff(Card.StatType stat, int amount)
    {
           switch (stat)
           {
            case Card.StatType.health:
                cardData.health += amount;
                break;
            case Card.StatType.damage:
                cardData.damage += amount;
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

