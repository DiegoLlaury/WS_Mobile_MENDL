using UnityEngine;
using WS_DiegoCo_Middle;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public CardMiddle cardMiddle;

    public HandManager handManager;
    public DeckManager deckManager;
    public EnemyManager enemyManager;
    public PlayerEvent player;

    private int cardStart = 4;
    private bool isPlayerTurn = true;

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
        StartBattle();
    }

    private void StartBattle()
    {
        player.ResetEnergy();
        handManager.cardsInHand.Clear();
        deckManager.ShuffleDeck();
        deckManager.DrawCard(cardStart);
    }

    public void EndPlayerTurn()
    {
        if (!isPlayerTurn) return;

        isPlayerTurn = false;

        player.ProcessTurnEffects();

        StartCoroutine(enemyManager.EnemyTurn());
    }

    public void EndEnemyTurn()
    {
        Debug.Log("Enemy Turn End");
        enemyManager.ProcessEnemyEffects();

        for (int i = handManager.cardsInHand.Count - 1; i >= 0; i--)
        {
            GameObject card = handManager.cardsInHand[i];
            Debug.Log($"Discarding card: {card.name}");
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
        if (cardMiddle.health <= 0)
        {
            Debug.Log("Game Over! You lost.");
        }
        else if (enemyManager.AllEnemiesDefeated())
        {
            Debug.Log("Victory! All enemies are dead.");
        }
    }
}
