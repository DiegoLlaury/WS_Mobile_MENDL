using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WS_DiegoCo;
using Unity.VisualScripting;

public class DeckManager : MonoBehaviour
{
    public List<Card> deck = new List<Card>();
    public List<Card> discardPile = new List<Card>();
    public int startCard = 4;
    public DropZone dropZone;
    public HandManager handManager;


    private void Start()
    {
        //Load all card assets from the Ressources folder
        Card[] cards = Resources.LoadAll<Card>("Cards");

        //Add the loaded cards to the allCards list
        deck.AddRange(cards);

        //Shuffle the deck
        ShuffleDeck();

        handManager = FindAnyObjectByType<HandManager>();
        
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
    public void DrawCard()
    {
       for (int i = 0; i < startCard; i++)
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
