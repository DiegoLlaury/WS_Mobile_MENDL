using UnityEngine;
using System.Collections;
using WS_DiegoCo_Event;

public class InvestigationMode : MonoBehaviour
{
    public static InvestigationMode Instance;
    public EnemyManager enemyManager;
    private EnemyDisplay suspectedMafioso;
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

    public void StartInvestigation(EventBattle investigationEvent)
    {
        enemyManager.StartCombat(investigationEvent);
        suspectedMafioso = enemyManager.GetRandomEnemy(); // Randomize the mafioso
        requiredInfiltration = investigationEvent.conditionNumber;
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

    public void CheckGameOver(PlayerEvent player)
    {
        if (suspectedMafioso.enemyData.perception <= player.cardData.perception)
        {
            GameManager.WinBattle = true;
            player.EndBattle();
        }
        else if (GameManager.currentEvent.currentTurn == 0 || player.cardData.health == 0)
        {
            GameManager.WinBattle = false;
            player.EndBattle();
            Debug.Log("You lost the investigation");
        }
    }
}
