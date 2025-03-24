using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WS_DiegoCo;

public class DeckManager : MonoBehaviour
{
    public enum CardType { Heart, Square, Spade, Clover }

    public List<Card> deck = new List<Card>();
    public List<Card> discardPile = new List<Card>();  
    public HandManager handManager;
    public PlayerEvent playerStats;

    private void Start()
    {
        handManager = FindAnyObjectByType<HandManager>();
        //playerStats = FindAnyObjectByType<CardMiddle>();

        if (playerStats == null)  //  Prevent errors if playerStats isn't found
        {
            Debug.LogError(" Error: CardMiddle (playerStats) is missing! Deck cannot be created.");
            return;
        }

        LoadDeckFromPlayerStats();
        //Shuffle the deck
        ShuffleDeck();       
    }

    private void LoadDeckFromPlayerStats()
    {
        deck.Clear();
        Debug.Log($"Heart: {playerStats.cardData.heart}, Square: {playerStats.cardData.square}, Spade: {playerStats.cardData.spade}, Clover: {playerStats.cardData.clover}");

        // Load cards based on their type folders
        Dictionary<Card.CardType, List<Card>> cardLibrary = new Dictionary<Card.CardType, List<Card>>();

        foreach (Card.CardType type in System.Enum.GetValues(typeof(Card.CardType)))
        {
            Card[] loadedCards = Resources.LoadAll<Card>($"Cards/{type}");

            if (loadedCards == null || loadedCards.Length == 0)
            {
                Debug.LogWarning($" No cards found in Resources/Cards/{type}!");
                continue;
            }

            cardLibrary[type] = new List<Card>(loadedCards);
        }

        // Get card counts from playerStats
        AddCardsToDeck(Card.CardType.Heart, playerStats.cardData.heart, cardLibrary);
        AddCardsToDeck(Card.CardType.Square, playerStats.cardData.square, cardLibrary);
        AddCardsToDeck(Card.CardType.Spade, playerStats.cardData.spade, cardLibrary);
        AddCardsToDeck(Card.CardType.Clover, playerStats.cardData.clover, cardLibrary);
        Debug.Log($"Total cards in deck: {deck.Count}");
        foreach (Card card in deck)
        {
            Debug.Log($"Card: {card.name} - Instance ID: {card.GetInstanceID()}");
        }
    }

    private void AddCardsToDeck(Card.CardType type, int count, Dictionary<Card.CardType, List<Card>> library)
    {
        if (!library.ContainsKey(type) || library[type].Count == 0)
        {
            Debug.LogWarning($" No valid cards available for {type}.");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            Card randomCard = library[type][Random.Range(0, library[type].Count)];
            deck.Add(randomCard);
        }
        Debug.Log($"Adding {count} {type} cards to the deck. Current deck size before: {deck.Count}");
    }

    public void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            Card temp = deck[i];
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
            Card nextCard = deck[0];
            deck.RemoveAt(0);
            handManager.AddCardToHand(nextCard);
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

    public void DiscardCard(Card card)
    {
        discardPile.Add(card);
    }
}
