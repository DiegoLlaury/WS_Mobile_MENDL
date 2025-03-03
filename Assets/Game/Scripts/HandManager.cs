using NUnit.Framework;
using UnityEngine;
using WS_DiegoCo;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine.InputSystem.XR.Haptics;

public class HandManager : MonoBehaviour
{
    public DeckManager deck;
    public CardMovement movement;
    public GameObject cardPrefab; //Assign card prefab in inspector
    public Transform handTransform; //Root of the hand position
    public Transform deckTransform;

    public float fanSpread = 5f;
    public float cardSpacing = 100f;
    public float verticalSpacing = 10f;
    public float moveDuration = 0.5f;

    public List<GameObject> cardsInHand = new List<GameObject>(); //Hold a list of th card objects in our hand
    private List<Vector3> cardsStartPos = new List<Vector3>();
    private List<Vector3> cardsTargetPos = new List<Vector3>();
    private List<Quaternion> cardsStartRot = new List<Quaternion>();
    private List<Quaternion> cardsTargetRot = new List<Quaternion>();

    public void AddCardToHand(Card cardData)
    {
        Debug.Log("TEST");

        //Instantiate the card
        
        GameObject newCard = Instantiate(cardPrefab, deckTransform.position, Quaternion.identity, handTransform);

        //Set the CardData of the instantiated card
        newCard.GetComponent<CardDisplay>().cardData = cardData;

        cardsInHand.Add(newCard);

        for (int i = 0; i < cardsInHand.Count-1; i++)
        {
            cardsTargetPos[i] = CalculateCardPosition(i);
            cardsTargetRot[i] = CalculateCardRotation(i);
        }

        cardsStartPos.Add(new Vector3());
        cardsStartRot.Add(new Quaternion());

        cardsTargetPos.Add(CalculateCardPosition(cardsInHand.Count - 1));
        cardsTargetRot.Add(CalculateCardRotation(cardsInHand.Count - 1));

        // Calculer la position finale avant l'animation

        StartCoroutine(MoveCardToHand());
    }

    private IEnumerator MoveCardToHand()
    {
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            cardsStartPos[i] = cardsInHand[i].transform.position;
            cardsStartRot[i] = cardsInHand[i].transform.rotation;
        }

        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            for (int i = 0; i < cardsInHand.Count; i++)
            {
                cardsInHand[i].transform.position = Vector3.Lerp(cardsStartPos[i], cardsTargetPos[i], t);
                cardsInHand[i].transform.rotation = Quaternion.Lerp(cardsStartRot[i], cardsTargetRot[i], t);
            }
            yield return null;
        }

        // S'assurer que la carte arrive bien à destination
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            cardsInHand[i].transform.position = cardsTargetPos[i];
            cardsInHand[i].transform.rotation = cardsTargetRot[i];
        }
    }

    private void Update()
    {

        
    }
    private Vector3 CalculateCardPosition(int cardIndex)
    {
        int cardCount = cardsInHand.Count;
        if (cardCount == 0) return handTransform.position;

        float horizontalOffset = cardSpacing * (cardIndex - (cardCount - 1) / 2f);

        float normalizedPosition = (cardCount > 1) ? (2f * cardIndex / (cardCount - 1) - 1f) : 0f;
        float verticalOffset = verticalSpacing * (1 - normalizedPosition * normalizedPosition);

        return handTransform.position + new Vector3(horizontalOffset, verticalOffset, 0f);
    }

    private Quaternion CalculateCardRotation(int cardIndex)
    {
        int cardCount = cardsInHand.Count;
        if (cardCount == 0) return Quaternion.Euler(0f, 0f, 0f);

        float rotationAngle = (fanSpread * (cardIndex - (cardCount - 1) / 2f));
        return Quaternion.Euler(0f, 0f, rotationAngle);
    }
}
