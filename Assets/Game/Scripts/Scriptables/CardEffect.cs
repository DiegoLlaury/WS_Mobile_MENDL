using UnityEngine;
using WS_DiegoCo;

public abstract class CardEffect : ScriptableObject, ICardEffect
{

    public abstract void ApplyEffect(EnemyDisplay enemy, Card cardData, PlayerEvent player, DeckManager deck, HandManager hand);
}
