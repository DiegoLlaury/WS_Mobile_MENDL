using UnityEngine;
using System.Collections;
using WS_DiegoCo_Event;

public class InfiltrationMode : MonoBehaviour
{
    public static InfiltrationMode Instance;
    public EnemyManager enemyManager;
  
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

    public void CheckGameOver(PlayerEvent player)
    {
        if (enemyManager.AllEnemiesDefeated() || player.cardData.discretion >= requiredInfiltration)
        {
            GameManager.WinBattle = true;
            player.EndBattle();
            Debug.Log("Infiltration Success!");
        }else if(GameManager.currentEvent.currentTurn == 0 || player.cardData.health == 0)
        {
            GameManager.WinBattle = false;
            player.EndBattle();
            Debug.Log("Infiltration Failed!");
        }
    }
}