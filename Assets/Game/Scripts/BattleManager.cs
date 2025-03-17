using UnityEngine;
using System.Collections;
using WS_DiegoCo_Middle;
using System.Collections.Generic;
using WS_DiegoCo_Event;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    public HandManager handManager;
    public DeckManager deckManager;
    public EnemyManager enemyManager;
    public PlayerEvent player;
    public InfiltrationMode infiltration;
    public InvestigationMode investigation;
    private GameManager game;

    private int cardStart = 4;
    private bool isPlayerTurn = true;
    private CardMiddle playerCard;
    private EventBattle currentEvent;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {

        game = GameManager.Instance;
       
        StartBattle();
        Debug.Log("Test");
    }

    private void StartBattle()
    {
        
        handManager.cardsInHand.Clear();
        deckManager.ShuffleDeck();
        StartPlayerTurn();

    }

    public void EndPlayerTurn()
    {
        if (!isPlayerTurn) return;

        isPlayerTurn = false;
        player.ProcessTurnEffects();
        CheckGameOver();
        player.TurnChange();
        StartCoroutine(enemyManager.EnemyTurn(game.currentEvent));
    }

    public void EndEnemyTurn()
    {
        Debug.Log("Enemy Turn End");
        enemyManager.ProcessEnemyEffects();

        for (int i = handManager.cardsInHand.Count - 1; i >= 0; i--)
        {
            GameObject card = handManager.cardsInHand[i];
            handManager.RemoveCardFromHand(card);
            deckManager.DiscardCard(card.GetComponent<CardDisplay>().cardData);
        }

        StartPlayerTurn();
    }

    private void StartPlayerTurn()
    {
        isPlayerTurn = true;
        player.ResetEnergy();
        deckManager.DrawCard(cardStart);
    }

    public void CheckGameOver()
    {
        switch (game.currentEvent.eventType)
        {
            case EventBattle.EventType.Combat:
                if (playerCard.health <= 0)
                {
                    Debug.Log("Game Over! You lost.");
                }
                else if (enemyManager.AllEnemiesDefeated())
                {
                    Debug.Log("Victory! All enemies are defeated.");
                }
                break;

            case EventBattle.EventType.Infiltration:
                infiltration.CheckGameOver();
                break;

            case EventBattle.EventType.Enquete:
                investigation.CheckGameOver();
                break;
        }
        
    }
}
