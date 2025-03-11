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

    private int cardStart = 4;
    private bool isPlayerTurn = true;
    private CardMiddle playerCard;
    private EventBattle battleEvent;

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
        LoadGameData();
        StartBattle();
    }

    private void LoadGameData()
    {
        playerCard = GameManager.Instance.selectedCard;
        battleEvent = GameManager.Instance.currentEvent;

        if (playerCard == null || battleEvent == null)
        {
            Debug.LogError("Donn�es du combat manquantes !");
            return;
        }
        enemyManager.StartCombat(battleEvent);
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
        if (playerCard.health <= 0)
        {
            Debug.Log("Game Over! You lost.");
        }
        else if (enemyManager.AllEnemiesDefeated())
        {
            Debug.Log("Victory! All enemies are defeated.");
        }
    }
}
