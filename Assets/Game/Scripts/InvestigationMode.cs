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
        requiredInfiltration = GameManager.currentEvent.conditionNumber;
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
        bool allEnemiesDetected = true;

        foreach (EnemyDisplay enemy in enemyManager.enemies)
        {
            if (player.cardData.perception < enemy.enemyData.discretion)
            {
                allEnemiesDetected = false;
                break; // Si un seul ennemi n'est pas détecté, pas besoin de continuer
            }
        }

        if (allEnemiesDetected && GameManager.currentEvent.currentTurn == 0)
        {
            GameManager.WinBattle = true;
            player.EndBattle();
            Debug.Log("Investigation réussie !");
        }
        else if (!allEnemiesDetected && GameManager.currentEvent.currentTurn == 0)
        {
            GameManager.WinBattle = false;
            player.EndBattle();
            Debug.Log("Vous avez perdu l'investigation !");
        }
    }
}
