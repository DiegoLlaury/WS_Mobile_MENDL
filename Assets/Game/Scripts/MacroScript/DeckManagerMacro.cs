using System.Collections.Generic;
using UnityEngine;
using WS_DiegoCo;
using WS_DiegoCo_Middle;

public class DeckManagerMacro : MonoBehaviour
{
    public HandManagerMacro handManagerMacro;

    public List<CardMiddle> deck = new List<CardMiddle>();
    public List<CardMiddle> discardPile = new List<CardMiddle>();

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
        CardMiddle[] cards = Resources.LoadAll<CardMiddle>("CardsMiddle");

        deck.AddRange(cards);
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
        for (int i = 0; i < cardNumber; i++)
        {

            if (deck.Count == 0)
            {
                RefillDeck();
            }

            if (deck.Count > 0)
            {
                CardMiddle nextCard = deck[0];
                deck.RemoveAt(0);
                handManagerMacro.AddCardToHand(nextCard);
            }
        }
    }

    private void RefillDeck()
    {
        if (discardPile.Count > 0)
        {
            deck.AddRange(discardPile);
            discardPile.Clear();
            ShuffleDeck();
            Debug.Log("Deck refilled with discarded cards.");
        }
    }

}
