using UnityEngine;
using WS_DiegoCo;

[CreateAssetMenu(fileName = "New Draw Effect", menuName = "Card Effects/Draw")]
public class DrawEffect : CardEffect
{
    public int numberOfCard;
    public enum TypeDraw
    {
        DrawANumber,

        DrawInfiltration
    }

    public TypeDraw typeDraw;

    public override void ApplyEffect(EnemyDisplay enemy, Card cardData, PlayerEvent player, DeckManager deck, HandManager hand, BattleManager battleManager, EnemyManager enemyManager)
    {
        switch (typeDraw)
        {
            case TypeDraw.DrawANumber:
                deck.DrawCard(numberOfCard);
                break;

            case TypeDraw.DrawInfiltration:
                int count = -1;

                foreach (GameObject cardObject in hand.cardsInHand)
                {
                    CardDisplay cardDisplay = cardObject.GetComponent<CardDisplay>();
                    if (cardDisplay != null && cardData.cardType == Card.CardType.Square)
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
