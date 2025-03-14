using UnityEngine;
using System.Collections;
using WS_DiegoCo_Event;

public class InvestigationMode : MonoBehaviour
{
    public static InvestigationMode Instance;
    public EnemyManager enemyManager;
    public PlayerEvent player;
    private EnemyDisplay suspectedMafioso;
    public GameManager game;

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

    public void StartInvestigation(EventBattle investigationEvent)
    {
        enemyManager.StartCombat(investigationEvent);
        suspectedMafioso = enemyManager.GetRandomEnemy(); // Randomize the mafioso
    }

    //public void EndPlayerTurn()
    //{
    //    turnsRemaining--;
    //    if (turnsRemaining <= 0)
    //    {
    //        Debug.Log("Investigation Failed! Time ran out.");
    //        return;
    //    }
    //    StartCoroutine(enemyManager.EnemyTurn());
    //}

    public void CheckGameOver()
    {
        if (suspectedMafioso.enemyData.perception <= 0)
        {
            Debug.Log("Investigation Success! The mafioso has been found.");
        }else if (game.currentEvent.currentTurn == 0)
        {
            Debug.Log("You lost the investigation");
        }
    }
}
