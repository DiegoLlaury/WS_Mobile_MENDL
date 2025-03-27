using UnityEngine;
using UnityEngine.UI;
using WS_DiegoCo_Middle;
using System.Collections.Generic;
using TMPro;

public class CardMiddleDisplay : MonoBehaviour
{
    public CardMiddle cardData;
    
    public Image cardImage;
    public Image backRedImage;
    public Image backBlackImage;

    public TMP_Text upLetter;
    public TMP_Text bottomLetter;

    public TMP_Text heartText;
    public TMP_Text squareText;
    public TMP_Text spadeText;
    public TMP_Text cloverText;

    public Image heartImage;
    public Image squareImage;
    public Image spadeImage;
    public Image cloverImage;

    public Transform gridMainStat;   // Référence à GridMainStat
    public Transform gridOtherStat;  // Référence à GridOtherStat
    public Transform gridMainImage;
    public Transform gridOtherImage;

    public Image[] typeImages;

    private void Start()
    {
        backBlackImage.gameObject.SetActive(false);
        backRedImage.gameObject.SetActive(false);
        cardData.health = cardData.maxHealth;
        cardData.strenght = cardData.maxStrenght;
        cardData.discretion = cardData.maxDiscretion;
        cardData.perception = cardData.maxPerception;
        if (!GameManager.generatedCharacter.Contains(cardData))
        {
            GenerateRandomStats();
            GameManager.generatedCharacter.Add(cardData);
            Debug.Log(cardData.cardName);
        }
        UpdateCardMiddle();
    }

    private void GenerateRandomStats()
    {

        int totalPoints = 30;
        int minStat = 3;
        int maxStat = 18;

        int skill = Random.Range(10, 30);

        // Détermine la stat principale (en fonction du symbole)
        int mainStat = Random.Range(13, 18); // Entre 10 et 15 pour un bon équilibre
        totalPoints -= mainStat;

        int[] stats = new int[4];

        // Assigne la stat principale
        switch (cardData.symbolTypes)
        {
            case CardMiddle.SymbolTypes.Heart:
                stats[0] = mainStat;
                break;
            case CardMiddle.SymbolTypes.Square:
                stats[1] = mainStat;
                break;
            case CardMiddle.SymbolTypes.Spade:
                stats[2] = mainStat;
                break;
            case CardMiddle.SymbolTypes.Clover:
                stats[3] = mainStat;
                break;
        }

        // Répartit le reste des points sur les autres stats (minimum 3, maximum 18)
        for (int i = 0; i < stats.Length; i++)
        {
            if (stats[i] == 0) // Si ce n'est pas la stat principale
            {
                int pointsToAdd = Random.Range(minStat, Mathf.Min(maxStat, totalPoints - (3 * (3 - i))) + 1);
                stats[i] = pointsToAdd;
                totalPoints -= pointsToAdd;
            }
        }

        // Si des points restent (arrondi ou autres), ajoute-les à une stat aléatoire non principale
        while (totalPoints > 0)
        {
            int randomIndex = Random.Range(0, 4);
            if (stats[randomIndex] < maxStat && stats[randomIndex] != mainStat)
            {
                stats[randomIndex]++;
                totalPoints--;
            }
        }

        // Assigne les stats générées
        cardData.heart = stats[0];
        cardData.square = stats[1];
        cardData.spade = stats[2];
        cardData.clover = stats[3];
    }

    private void UpdateCardMiddle()
    {
        heartText.text = cardData.heart.ToString();
        squareText.text = cardData.square.ToString();
        spadeText.text = cardData.spade.ToString();
        cloverText.text = cardData.clover.ToString();

        // Réinitialise la hiérarchie
        heartText.transform.SetParent(gridOtherStat);
        squareText.transform.SetParent(gridOtherStat);
        spadeText.transform.SetParent(gridOtherStat);
        cloverText.transform.SetParent(gridOtherStat);
        heartImage.transform.SetParent(gridOtherImage);
        squareImage.transform.SetParent(gridOtherImage);
        spadeImage.transform.SetParent(gridOtherImage);
        cloverImage.transform.SetParent(gridOtherImage);

        // Place la stat principale dans GridMainStat
        switch (cardData.symbolTypes)
        {
            case CardMiddle.SymbolTypes.Heart:
                heartText.transform.SetParent(gridMainStat);
                spadeText.transform.SetSiblingIndex(0);
                squareText.transform.SetSiblingIndex(1);
                cloverText.transform.SetSiblingIndex(2);

                heartImage.transform.SetParent(gridMainImage);
                spadeImage.transform.SetSiblingIndex(0);
                squareImage.transform.SetSiblingIndex(1);
                cloverImage.transform.SetSiblingIndex(2);
                
                backRedImage.gameObject.SetActive(true);
                break;

            case CardMiddle.SymbolTypes.Square:
                squareText.transform.SetParent(gridMainStat);
                spadeText.transform.SetSiblingIndex(0);
                heartText.transform.SetSiblingIndex(1);
                cloverText.transform.SetSiblingIndex(2);

                squareImage.transform.SetParent(gridMainImage);
                spadeImage.transform.SetSiblingIndex(0);
                heartImage.transform.SetSiblingIndex(1);
                cloverImage.transform.SetSiblingIndex(2);

                backRedImage.gameObject.SetActive(true);
                break;

            case CardMiddle.SymbolTypes.Spade:
                spadeText.transform.SetParent(gridMainStat);
                heartText.transform.SetSiblingIndex(0);
                cloverText.transform.SetSiblingIndex(1);
                squareText.transform.SetSiblingIndex(2);

                spadeImage.transform.SetParent(gridMainImage);
                heartImage.transform.SetSiblingIndex(0);
                cloverImage.transform.SetSiblingIndex(1);
                squareImage.transform.SetSiblingIndex(2);

                backBlackImage.gameObject.SetActive(true);

                break;

            case CardMiddle.SymbolTypes.Clover:
                cloverText.transform.SetParent(gridMainStat);
                heartText.transform.SetSiblingIndex(0);
                spadeText.transform.SetSiblingIndex(1);
                squareText.transform.SetSiblingIndex(2);

                cloverImage.transform.SetParent(gridMainImage);
                heartImage.transform.SetSiblingIndex(0);
                spadeImage.transform.SetSiblingIndex(1);
                squareImage.transform.SetSiblingIndex(2);

                backBlackImage.gameObject.SetActive(true);
                break;
        }

        cardImage.sprite = cardData.cardImage;
        if (cardData.symbolTypes == CardMiddle.SymbolTypes.Heart || cardData.symbolTypes == CardMiddle.SymbolTypes.Square)
        {
            cardImage.color = new Color32(173, 3, 3, 255);
        }
        else
        {
            cardImage.color = Color.black;
        }

        upLetter.text = cardData.letter;
        bottomLetter.text = cardData.letter;
        if (cardData.symbolTypes == CardMiddle.SymbolTypes.Heart || cardData.symbolTypes == CardMiddle.SymbolTypes.Square)
        {
            upLetter.color = new Color32(173, 3, 3, 255);
            bottomLetter.color = new Color32(173, 3, 3, 255);
        }
        else
        {
            upLetter.color = Color.black;
            bottomLetter.color = Color.black;
        }


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
