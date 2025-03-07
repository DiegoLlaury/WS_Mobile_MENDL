using UnityEngine;
using WS_DiegoCo;

[CreateAssetMenu(fileName = "New Draw Effect", menuName = "Card Effects/Draw")]
public class DrawEffect : CardEffect
{
    public int numberOfCard;
    public enum TypeDraw
    {
        DrawANumber,

        DrawHand,

        DrawInfiltration
    }

    public TypeDraw typeDraw;

    public override void ApplyEffect(EnemyDisplay enemy, Card cardData, PlayerEvent player, DeckManager deck, HandManager hand, GameManager gameManager, EnemyManager enemyManager)
    {
        switch (typeDraw)
        {
            case TypeDraw.DrawANumber:
                deck.DrawCard(numberOfCard);
                break;

            case TypeDraw.DrawHand:
                deck.DrawCard(hand.cardsInHand.Count);
                break;

            case TypeDraw.DrawInfiltration:
                int count = 0;

                foreach (GameObject cardObject in hand.cardsInHand)
                {
                    CardDisplay cardDisplay = cardObject.GetComponent<CardDisplay>();
                    if (cardDisplay != null && cardData.cardType.Contains(Card.CardType.Square))
                    {
                        count++;
                    }
                }

                int drawAmount = Mathf.Min(count, 3);
                deck.DrawCard(drawAmount);
                break;

        }
        
        
    }
}
