using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using WS_DiegoCo;
using System;

public class CardDisplay : MonoBehaviour
{

    public Card cardData;

    public Image cardImage;
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
            { Card.StatType.shield, shieldText }
        };
    }

    public void Start()
    {
        UpdateCardDisplay();
    }

    private void UpdateCardDisplay()
    {

        nameText.text = cardData.cardName;
        energyText.text = cardData.energy.ToString();
        dropTypeText.text = string.Join(", ", cardData.dropType);

        // Update stat text fields
        string formattedEffect = cardData.effect;
        foreach (var stat in statTexts)
        {
            string placeholder = "{" + stat.Key.ToString().ToLower() + "}"; // Example: "{health}"
            string statValue = GetStatValue(stat.Key).ToString();
            formattedEffect = formattedEffect.Replace(placeholder, statValue);
        }

        effectText.text = formattedEffect;

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

        //Update type images
        for (int i = 0; i < typeImages.Length; i++)
        {
            if (i < cardData.cardType.Count)
            {
                typeImages[i].gameObject.SetActive(true);
            }
            else
            {
                typeImages[i].gameObject.SetActive(false);
            }
        }

       
    }

    private int GetStatValue(Card.StatType statType)
    {
        return statType switch
        {
            Card.StatType.health => cardData.health,
            Card.StatType.shield => cardData.shield,
            Card.StatType.damage => cardData.damage,
            Card.StatType.discretion => cardData.discretion,
            Card.StatType.perception => cardData.perception,
            _ => 0
        };
    }
}
