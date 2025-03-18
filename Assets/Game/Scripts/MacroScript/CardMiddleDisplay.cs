using UnityEngine;
using UnityEngine.UI;
using WS_DiegoCo_Middle;
using System.Collections.Generic;
using TMPro;

public class CardMiddleDisplay : MonoBehaviour
{
    public CardMiddle cardData;
    
    public Image cardImage;
    public TMP_Text nameCardText;
    public TMP_Text heartText;
    public TMP_Text squareText;
    public TMP_Text spadeText;
    public TMP_Text cloverText;
    public Image[] typeImages;

    private void Start()
    {
        cardData.health = cardData.maxHealth;
        cardData.strenght = cardData.maxStrenght;
        cardData.discretion = cardData.maxDiscretion;
        cardData.perception = cardData.maxPerception;
        UpdateCardMiddle();
    }

    private void UpdateCardMiddle()
    {
        nameCardText.text = cardData.cardName;
        heartText.text = cardData.heart.ToString();
        squareText.text = cardData.square.ToString();
        spadeText.text = cardData.spade.ToString();
        cloverText.text = cardData.clover.ToString();

        foreach (Image img in typeImages)
        {
            img.gameObject.SetActive(false); // Deactivate all type images first
        }

        int typeIndex = (int)cardData.symbolTypes;
        if (typeIndex >= 0 && typeIndex < typeImages.Length)
        {
            typeImages[typeIndex].gameObject.SetActive(true);
        }
    }
}
