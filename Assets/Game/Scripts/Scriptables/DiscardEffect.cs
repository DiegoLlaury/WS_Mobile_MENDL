using System.Collections.Generic;
using UnityEngine;
using WS_DiegoCo;


[CreateAssetMenu(fileName = "New Discard Effect", menuName = "Card Effects/Discard")]
public class DiscardEffect : CardEffect
{

    public enum DiscardType
    {
        DiscardByType,      // Discard all cards of a specific type (e.g., Spades, Clubs, Diamonds)
        DiscardRandom,      // Discard a random card
        DiscardAll,         // Discard the entire hand
        DiscardNumber      // Discard one
    }

    public int discardNumber;
    public DiscardType discardType;
    public Card.CardType targetCardType;
    public int gainHealthPerCard = 0;    // Used when gaining health per discarded card
    public int gainInfiltrationPerCard = 0; // Used when gaining infiltration per discarded card
    public int gainPerceptionPerCard = 0;   // Used when gaining perception per discarded card
    public int drawAmountAfterDiscard = 0;  // Used when drawing after discard


    public override void ApplyEffect(EnemyDisplay enemy, Card cardData, PlayerEvent player, DeckManager deck, HandManager hand)
    {
        List<GameObject> cardsToDiscard = new List<GameObject>();

        switch (discardType)
        {
            case DiscardType.DiscardByType:
                // Find all cards of the specified type
                foreach (GameObject cardObject in hand.cardsInHand)
                {
                    CardDisplay cardDisplay = cardObject.GetComponent<CardDisplay>();

                    if (cardDisplay != null && cardDisplay.cardData.cardType.Contains(targetCardType))
                    {
                        cardsToDiscard.Add(cardObject);
                    }
                }
                break;

            case DiscardType.DiscardRandom:
                if (hand.cardsInHand.Count > 0)
                {
                    int randomIndex = Random.Range(0, hand.cardsInHand.Count);
                    cardsToDiscard.Add(hand.cardsInHand[randomIndex]);
                }
                break;

            case DiscardType.DiscardAll:
                cardsToDiscard.AddRange(hand.cardsInHand);
                break;

            case DiscardType.DiscardNumber:
                if (hand.cardsInHand.Count > 0)
                {
                    int randomIndex = Random.Range(0, hand.cardsInHand.Count);
                    cardsToDiscard.Add(hand.cardsInHand[randomIndex]);
                    deck.DrawCard(1); // Draw one card after discarding
                }
                break;
        }

        // Apply discard effect
        foreach (GameObject card in cardsToDiscard)
        {
            hand.RemoveCardFromHand(card);
        }

        int discardedCount = cardsToDiscard.Count;

        // Apply rewards for discarded cards
        if (gainHealthPerCard > 0) player.GainHealth(discardedCount * gainHealthPerCard);
        if (gainInfiltrationPerCard > 0) player.GainInfiltration(discardedCount * gainInfiltrationPerCard);
        if (gainPerceptionPerCard > 0) player.GainPerception(discardedCount * gainPerceptionPerCard);

        // Draw cards after discard if needed
        if (discardType == DiscardType.DiscardAll)
        {
            deck.DrawCard(discardedCount);
        }
        else if (drawAmountAfterDiscard > 0)
        {
            deck.DrawCard(drawAmountAfterDiscard);
        }

        Debug.Log($"Discarded {discardedCount} cards. Effects applied.");
    }
}
