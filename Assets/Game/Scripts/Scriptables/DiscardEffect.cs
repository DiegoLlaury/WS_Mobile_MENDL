using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using WS_DiegoCo;

[CreateAssetMenu(fileName = "New Discard Effect", menuName = "Card Effects/Discard")]
public class DiscardEffect : CardEffect
{
    public enum DiscardType
    {
        DiscardByType,  // Discard all cards of a specific type
        DiscardRandom,  // Discard a random card
        DiscardAll,     // Discard the entire hand
        DiscardNumber   // Discard a specific number of random cards
    }

    public int discardNumber = 1;
    public DiscardType discardType;
    public Card.CardType targetCardType;
    public int gainHealthPerCard = 0;
    public int gainInfiltrationPerCard = 0;
    public int gainPerceptionPerCard = 0;
    public int drawAmountAfterDiscard = 0;

    public override void ApplyEffect(EnemyDisplay enemy, Card cardData, PlayerEvent player, DeckManager deck, HandManager hand, BattleManager battleManager, EnemyManager enemyManager)
    {
        List<GameObject> cardsToDiscard = new List<GameObject>();

        switch (discardType)
        {
            case DiscardType.DiscardByType:
                cardsToDiscard = GetCardsByType(hand, targetCardType);
                break;

            case DiscardType.DiscardRandom:
                cardsToDiscard = GetRandomCards(hand, 1, cardData);
                break;

            case DiscardType.DiscardAll:
                cardsToDiscard.AddRange(hand.cardsInHand);
                drawAmountAfterDiscard = cardsToDiscard.Count-1;
                break;

            case DiscardType.DiscardNumber:
                cardsToDiscard = GetRandomCards(hand, 1, cardData);
                break;
        }

        int discardedCount = 0;

        // Discard and move cards to discard pile
        foreach (GameObject cardObj in cardsToDiscard)
        {
            CardDisplay display = cardObj.GetComponent<CardDisplay>();
            if (display != null)
            {
                Debug.Log(display.cardData.name);
                if (display.cardData != cardData)
                {
                    deck.DiscardCard(display.cardData); // Move the actual card data to the discard pile
                }
            }
            hand.RemoveCardFromHand(cardObj); // Properly removes from hand
            discardedCount++;
        }

        // Apply bonuses for discarded cards
        if (gainHealthPerCard > 0) player.GainHealth(discardedCount * gainHealthPerCard);
        if (gainInfiltrationPerCard > 0) player.GainInfiltration(discardedCount * gainInfiltrationPerCard);
        if (gainPerceptionPerCard > 0) player.GainPerception(discardedCount * gainPerceptionPerCard);

        // Draw new cards if needed
        if (drawAmountAfterDiscard > 0)
        {
            deck.DrawCard(drawAmountAfterDiscard);
        }
        else if (discardType == DiscardType.DiscardAll)
        {
            deck.DrawCard(discardedCount);
        }

        Debug.Log($"Discarded {discardedCount} cards. Effects applied.");
    }

    private List<GameObject> GetCardsByType(HandManager hand, Card.CardType type)
    {
        List<GameObject> result = new List<GameObject>();
        foreach (GameObject cardObj in hand.cardsInHand)
        {
            CardDisplay display = cardObj.GetComponent<CardDisplay>();
            if (display != null && display.cardData.cardType == type)
            {
                result.Add(cardObj);
            }
        }
        return result;
    }

    private List<GameObject> GetRandomCards(HandManager hand, int count, Card cardData)
    {
        List<GameObject> result = new List<GameObject>(hand.cardsInHand);
        List<GameObject> selected = new List<GameObject>();
        
        while (selected.Count != count)
        {
            int randomIndex = Random.Range(0, result.Count - 1);
            if (result[randomIndex].GetComponent<CardDisplay>().cardData != cardData)
            {
                selected.Add(result[randomIndex]);
                result.RemoveAt(randomIndex);
            }

            if (result.Count <= 1)
            {
                return selected;
            }
        }
        return selected;
    }
}