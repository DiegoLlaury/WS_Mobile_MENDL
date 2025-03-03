using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WS_DiegoCo;
using Unity.VisualScripting;

public class DeckManager : MonoBehaviour
{
    public List<Card> deck = new List<Card>();
    public int startCard = 4;
    

    private void Start()
    {
        //Load all card assets from the Ressources folder
        Card[] cards = Resources.LoadAll<Card>("Cards");

        //Add the loaded cards to the allCards list
        deck.AddRange(cards);

        //Shuffle the deck
        ShuffleDeck();

        HandManager hand = FindAnyObjectByType<HandManager>();
        for (int i = 0; i < startCard; i++)
        {
            DrawCard(hand);
        }
    }

    private void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            Card temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }
    public void DrawCard(HandManager handManager)
    {
        if (deck.Count == 0)
        return;

        int randomIndex = Random.Range(0, deck.Count);
        Card nextCard = deck[randomIndex];
        
        //Add a card
        handManager.AddCardToHand(nextCard);

        //Remove from the deck
        deck.RemoveAt(randomIndex);
    }
}
