using UnityEngine;
using WS_DiegoCo_Middle;
using System.Collections;
using System.Collections.Generic;

public class HandManagerMacro : MonoBehaviour
{
    public DeckManagerMacro deck;
    public GameObject cardPrefab; //Assign card prefab in inspector
    public Transform handTransform; //Root of the hand position
    public Transform deckTransform;

    public float fanSpread = 5f;
    public float cardSpacing = 100f;
    public float verticalSpacing = 10f;
    public float moveDuration = 0.5f;

    public List<GameObject> cardsInHand = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        UpdateCardPositions();
    }

    public void AddCardToHand(CardMiddle cardData)
    {
        // Instantiate the card at the deck's position
        GameObject newCard = Instantiate(cardPrefab, deckTransform.position, Quaternion.identity, handTransform);

        // Assign card data
        CardMiddleDisplay display = newCard.GetComponent<CardMiddleDisplay>();

        // Add to hand
        cardsInHand.Add(newCard);
        //display.UpdateAllCards();
        // Recalculate positions and animate movement
        UpdateCardPositions();
    }

    private void UpdateCardPositions()
    {
        int cardCount = cardsInHand.Count;
        if (cardCount == 0) return;

        for (int i = 0; i < cardCount; i++)
        {
            Vector3 targetPos = CalculateCardPosition(i, cardCount);
            Quaternion targetRot = CalculateCardRotation(i, cardCount);
            StartCoroutine(AnimateCardMovement(cardsInHand[i], targetPos, targetRot));
        }
    }

    private IEnumerator AnimateCardMovement(GameObject card, Vector3 targetPos, Quaternion targetRot)
    {
        if (card == null)
        {
            Debug.LogWarning("AnimateCardMovement: Card is null before animation starts.");
            yield break;
        }

        float elapsedTime = 0f;
        Vector3 startPos = card.transform.position;
        Quaternion startRot = card.transform.rotation;

        while (elapsedTime < moveDuration)
        {
            if (card == null) // Check if the card has been destroyed mid-animation
            {
                yield break;
            }

            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;

            card.transform.position = Vector3.Lerp(startPos, targetPos, t);
            card.transform.rotation = Quaternion.Lerp(startRot, targetRot, t);

            yield return null;
        }
        if (card != null)
        {
            card.transform.position = targetPos;
            card.transform.rotation = targetRot;
        } // Final check before setting position{           
    }

    private Vector3 CalculateCardPosition(int index, int totalCards)
    {
        float horizontalOffset = cardSpacing * (index - (totalCards - 1) / 2f);
        float normalizedPosition = (totalCards > 1) ? (2f * index / (totalCards - 1) - 1f) : 0f;
        float verticalOffset = verticalSpacing * (1 - normalizedPosition * normalizedPosition);

        return handTransform.position + new Vector3(horizontalOffset, verticalOffset, 0f);
    }

    private Quaternion CalculateCardRotation(int index, int totalCards)
    {
        float rotationAngle = fanSpread * (index - (totalCards - 1) / 2f);
        return Quaternion.Euler(0f, 0f, rotationAngle);
    }
}
