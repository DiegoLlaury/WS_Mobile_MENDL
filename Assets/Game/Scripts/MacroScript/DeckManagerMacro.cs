using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WS_DiegoCo;
using WS_DiegoCo_Middle;

public class DeckManagerMacro : MonoBehaviour
{
    public HandManagerMacro handManagerMacro;

    public List<CardMiddle> deck = new List<CardMiddle>();
    public List<CardMiddle> cardMiddleInEvent = new List<CardMiddle>();
    private float drawDelay = 0.2f;

    private int drawCard = 4;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        handManagerMacro = FindAnyObjectByType<HandManagerMacro>();
        LoadDeck();
        DrawCard(drawCard);
    }

    private void LoadDeck()
    {
        deck.Clear();
        CardMiddle[] cards = Resources.LoadAll<CardMiddle>("CardsMiddle");


        foreach (CardMiddle card in cards)
        {
            if (!deck.Contains(card))
            {
                deck.Add(card);
            }
        }

        foreach (CardMiddle card in deck)
        {
            Debug.Log($"Card: {card.name} - Instance ID: {card.GetInstanceID()}");
        }
    }

    public void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            CardMiddle temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    public void DrawCard(int cardNumber)
    {
        StartCoroutine(DrawCardOneByOne(cardNumber));
    }

    private IEnumerator DrawCardOneByOne(int cardNumber)
    {
        for (int i = 0; i < cardNumber; i++)
        {
            if (deck.Count == 0)
            {
                // RefillDeck(); // Décommente cette ligne si tu veux reshuffle le deck
                break;
            }

            if (deck.Count > 0)
            {
                CardMiddle nextCard = deck[0];
                deck.RemoveAt(0);
                handManagerMacro.AddCardToHand(nextCard);
            }

            yield return new WaitForSeconds(drawDelay);
        }
    }

    //private void RefillDeck()
    //{
    //    if (discardPile.Count > 0)
    //    {
    //        deck.AddRange(discardPile);
    //        discardPile.Clear();
    //        ShuffleDeck();
    //        Debug.Log("Deck refilled with discarded cards.");
    //    }
    //}
    public void StockCard(CardMiddle card)
    {
        if (!cardMiddleInEvent.Contains(card))
        {
            cardMiddleInEvent.Add(card);
        }
        else
        {
            Debug.LogWarning($"La carte {card.cardName} est déjà dans l'événement.");
        }
    }

    public void UnstockCard(CardMiddle card)
    {
        if (cardMiddleInEvent.Contains(card))
        {
            cardMiddleInEvent.Remove(card);
        }
    }
}
