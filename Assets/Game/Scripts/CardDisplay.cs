using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using WS_DiegoCo;
using WS_DiegoCo_Middle;
using System;
using Unity.VisualScripting;

public class CardDisplay : MonoBehaviour
{

    public Card cardData;
    private CardMiddle cardMiddle;
    private PlayerEvent player;

    public Image cardImage;
    public Image energyRed1;
    public Image energyRed2;
    public Image energyRed3;
    public Image energyBlack1;
    public Image energyBlack2;
    public Image energyBlack3;
    public TMP_Text nameText;
    public TMP_Text effectText;

    public TMP_Text healthText;
    public TMP_Text defenseText;
    public TMP_Text damageText;
    public TMP_Text energyText;
    public TMP_Text perceptionText;
    public TMP_Text discretionText;
    public TMP_Text shieldText;

    public TMP_Text dropTypeText;

    public Image[] typeImages;

    private Dictionary<Card.StatType, TMP_Text> statTexts;

    void Awake()
    {
        // Initialize dictionary for easy lookup
        statTexts = new Dictionary<Card.StatType, TMP_Text>
        {
            { Card.StatType.health, healthText },
            { Card.StatType.defense, defenseText },
            { Card.StatType.damage, damageText },
            { Card.StatType.discretion, discretionText },
            { Card.StatType.perception, perceptionText },
        };
    }

    public void Start()
    {
        
        player = FindAnyObjectByType<PlayerEvent>();
        if (player == null)
        {
            Debug.LogError("PlayerEvent not found! Make sure a PlayerEvent exists in the scene.");
        }

        cardData.damage = cardData.startingDamage + player.cardData.strenght;
        UpdateCardDisplay();
    }


    private void UpdateCardDisplay()
    {

        nameText.text = cardData.cardName;
        cardImage.sprite = cardData.cardImage;
        cardImage.color = Color.white;
        // Update stat text fields
        string formattedEffect = cardData.effect;
        foreach (var stat in statTexts)
        {
            string placeholder = "{" + stat.Key.ToString().ToLower() + "}"; // Example: "{health}"
            string statValue = GetStatValue(stat.Key).ToString();
            formattedEffect = formattedEffect.Replace(placeholder, statValue);
        }

        effectText.text = formattedEffect;

        if (cardData.cardType == Card.CardType.Heart || cardData.cardType == Card.CardType.Square)
        {
            nameText.color = new Color32(173, 3, 3, 255); // #AD0303
            effectText.color = new Color32(173, 3, 3, 255); // #AD0303
        }
        else
        {
            nameText.color = Color.black; // Default color
            effectText.color = Color.black;
        }

        foreach (var stat in statTexts)
        {
            stat.Value.text = GetStatValue(stat.Key).ToString();
            stat.Value.gameObject.SetActive(false);
        }

        foreach (WS_DiegoCo.Card.StatType statType in cardData.statType)
        {
            if (statTexts.ContainsKey(statType))
            {
                statTexts[statType].gameObject.SetActive(true);
            }
        }

        foreach (Image img in typeImages)
        {
            img.gameObject.SetActive(false); // Deactivate all type images first
        }

        int typeIndex = (int)cardData.cardType;
        if (typeIndex >= 0 && typeIndex < typeImages.Length)
        {
            typeImages[typeIndex].gameObject.SetActive(true);
        }
        switch (cardData.cardType)
        {
            case Card.CardType.Heart:
                UpdateEnergyRed();
                break;

            case Card.CardType.Spade:
                UpdateEnergyBlack();
                break;

            case Card.CardType.Square:
                UpdateEnergyRed();
                break;

            case Card.CardType.Clover:
                UpdateEnergyBlack();
                break;
        }
        
    }

    private void UpdateEnergyRed()
    {
        switch (cardData.energy)
        {

            case 0:
                break;

            case 1:
                energyRed1.gameObject.SetActive(true);
                break;

            case 2:
                energyRed1.gameObject.SetActive(true);
                energyRed2.gameObject.SetActive(true);
                break;

            case 3:
                energyRed1.gameObject.SetActive(true);
                energyRed2.gameObject.SetActive(true);
                energyRed3.gameObject.SetActive(true);
                break;
        }
    }

    private void UpdateEnergyBlack()
    {
        switch (cardData.energy)
        {

            case 0:
                break;

            case 1:
                energyBlack1.gameObject.SetActive(true);
                break;

            case 2:
                energyBlack1.gameObject.SetActive(true);
                energyBlack2.gameObject.SetActive(true);
                break;

            case 3:
                energyBlack1.gameObject.SetActive(true);
                energyBlack2.gameObject.SetActive(true);
                energyBlack3.gameObject.SetActive(true);
                break;
        }
    }

    public void UpdateDamage(int strength)
    {
        cardData.damage = cardData.startingDamage + strength;
        Debug.Log("Damage Update, Card damage : " + cardData.damage +", Player strength : "+ player.cardData.strenght);
        UpdateCardDisplay();
    }

    public static void UpdateAllCards(int strength)
    {
        CardDisplay[] cards = FindObjectsByType<CardDisplay>(FindObjectsSortMode.None);
        foreach (CardDisplay card in cards)
        {
            card.UpdateDamage(strength);
        }
    }

    private int GetStatValue(Card.StatType statType)
    {
        return statType switch
        {
            Card.StatType.health => cardData.health,
            Card.StatType.defense => cardData.defense,
            Card.StatType.damage => cardData.damage,
            Card.StatType.discretion => cardData.discretion,
            Card.StatType.perception => cardData.perception,
            _ => 0
        };
    }
}
