using UnityEngine;
using System.Collections;
using WS_DiegoCo_Event;

public class InfiltrationMode : MonoBehaviour
{
    public static InfiltrationMode Instance;
    public EnemyManager enemyManager;
    public PlayerEvent player;
    public GameManager game;
    private int infiltrationScore;
    private int requiredInfiltration;

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

    public void StartInfiltration(EventBattle infiltrationEvent)
    {
        enemyManager.StartCombat(infiltrationEvent);
        requiredInfiltration = infiltrationEvent.conditionNumber;
        infiltrationScore = player.cardData.discretion;
    }

    //public void EndPlayerTurn()
    //{
    //    if (enemyManager.AllEnemiesDefeated() || infiltrationScore >= requiredInfiltration)
    //    {
    //        Debug.Log("Infiltration Success!");
    //        return;
    //    }
    //    else
    //    {
    //        StartCoroutine(enemyManager.EnemyTurn());
    //    }
    //}

    public void CheckGameOver()
    {
        if (enemyManager.AllEnemiesDefeated() || infiltrationScore >= requiredInfiltration)
        {
            Debug.Log("Infiltration Success!");
        }else if(game.currentEvent.currentTurn == 0)
        {
            Debug.Log("Infiltration Failed!");
        }
    }
}