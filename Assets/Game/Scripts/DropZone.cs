using System.Collections.Generic;
using UnityEngine;
using WS_DiegoCo;


public class DropZone : MonoBehaviour
{
    public List<Card> discardedCards = new List<Card>(); // List to store used cards

    public void AddCardToDiscard(Card card)
    {
        discardedCards.Add(card);
    }

    public List<Card> GetDiscardedCards()
    {
        return discardedCards;
    }

    public void ClearDiscardPile()
    {
        discardedCards.Clear();
    }
}
