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


    private int cardStart = 4;
    private bool isPlayerTurn = true;
    private CardMiddle playerCard;
    private EventBattle currentEvent;

    public AudioSource DrawSound;

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
        
        handManager.cardsInHand.Clear();
        deckManager.ShuffleDeck();
        StartPlayerTurn();

    }
    private void StartPlayerTurn()
    {
        isPlayerTurn = true;
        player.ResetEnergy();
        deckManager.DrawCard(cardStart);
        DrawSound.Play();
        enemyManager.GenerateEnemyActions(GameManager.currentEvent.eventDifficulty, GameManager.currentEvent.eventType);
    }

    public void EndPlayerTurn()
    {
        if (!isPlayerTurn) return;

        isPlayerTurn = false;
        for (int i = handManager.cardsInHand.Count - 1; i >= 0; i--)
        {
            GameObject card = handManager.cardsInHand[i];
            handManager.RemoveCardFromHand(card);
            deckManager.DiscardCard(card.GetComponent<CardDisplay>().cardData);
        }
        //player.ProcessTurnEffects();
        CheckGameOver();
        player.TurnChange();
        StartCoroutine(enemyManager.EnemyTurn(GameManager.currentEvent));
        
    }

    public void EndEnemyTurn()
    {
        Debug.Log("Enemy Turn End");
        enemyManager.ProcessEnemyEffects();

        StartPlayerTurn();
    }



    public void CheckGameOver()
    {
        switch (GameManager.currentEvent.eventType)
        {
            case EventBattle.EventType.Combat:
                if (player.cardData.health <= 0)
                {
                    GameManager.WinBattle = false;
                    player.EndBattle();
                }
                else if (enemyManager.AllEnemiesDefeated())
                {
                    GameManager.WinBattle = true;
                    player.EndBattle();
                }
                break;

            case EventBattle.EventType.Infiltration:
                infiltration.CheckGameOver(player);
                break;

            case EventBattle.EventType.Enquete:
                investigation.CheckGameOver(player);
                break;
        }      
    }
}
