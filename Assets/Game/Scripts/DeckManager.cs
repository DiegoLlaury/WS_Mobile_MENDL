using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WS_DiegoCo;

public class DeckManager : MonoBehaviour
{
    public List<Card> allCards = new List<Card>();

    private int currentIndex = 0;

    public void DrawCard(HandManager handManager)
    {
        if (allCards.Count == 0)
        return;

        Card nextCard = allCards[currentIndex];
        handManager.AddCardToHand(nextCard);
        currentIndex = (currentIndex + 1) % allCards.Count;
    }
}
