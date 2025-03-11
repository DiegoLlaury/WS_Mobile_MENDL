using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WS_DiegoCo_Middle;
using WS_DiegoCo_Event;
using WS_DiegoCo_Enemy;
using WS_DiegoCo;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    public List<EnemyDisplay> enemies = new List<EnemyDisplay>();

    public Transform enemySpawnPoint;
    public GameObject enemyPrefab;

    private float spacing = 2.5f;

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

    public void StartCombat(EventBattle combatEvent)
    {
        if (combatEvent.eventType != EventBattle.EventType.Combat)
        {
            Debug.LogError("Tried to start combat with a non-combat event!");
            return;
        }

        ClearEnemies();

        for (int i = 0; i < combatEvent.enemies.Count; i++)
        {
            SpawnEnemy(combatEvent.enemies[i], i);
        }
    }

    private void SpawnEnemy(Enemy enemyData, int index)
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab is missing!");
            return;
        }

        Vector3 spawnPosition = enemySpawnPoint.position + new Vector3(index * spacing, 0, 0);
        GameObject newEnemyObj = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, enemySpawnPoint);

        EnemyDisplay enemyDisplay = newEnemyObj.GetComponent<EnemyDisplay>();
        if (enemyDisplay != null)
        {
            enemyDisplay.Initialize(enemyData);
            enemies.Add(enemyDisplay);
        }
        else
        {
            Debug.LogError("Enemy prefab is missing EnemyDisplay component!");
        }
    }

    public IEnumerator EnemyTurn()
    {
        Debug.Log("Enemy Turn Start");
        List<EnemyDisplay> enemiesToRemove = new List<EnemyDisplay>();

        foreach (EnemyDisplay enemy in enemies)
        {
            if (enemy.enemyData.health > 0)
            {
                enemy.ProcessTurnEffects();
                if (enemy.enemyData.health <= 0)
                {
                    enemiesToRemove.Add(enemy);
                    continue;
                }

                PerformEnemyAction(enemy);
                yield return new WaitForSeconds(2f);
            }
        }

        foreach (EnemyDisplay enemy in enemiesToRemove)
        {
            RemoveEnemy(enemy);
        }

        BattleManager.Instance.EndEnemyTurn();
    }

    private void PerformEnemyAction(EnemyDisplay enemy)
    {
        int actionType = Random.Range(0, 3); // 0 = Attack, 1 = Buff, 2 = Debuff

        switch (actionType)
        {
            case 0: // Attack
                BattleManager.Instance.player.TakeDamage(enemy.enemyData.damage);
                Debug.Log($"{enemy.enemyData.enemyName} attacks for {enemy.enemyData.damage} damage!");
                break;

            case 1: // Buff
                enemy.ModifyStat(Card.StatType.damage, 1);
                Debug.Log($"{enemy.enemyData.enemyName} buffs its damage!");
                break;

            case 2: // Debuff (weaken player)
                BattleManager.Instance.player.ApplyDebuff(Card.StatType.damage, -1);
                Debug.Log($"{enemy.enemyData.enemyName} weakens the player!");
                break;
        }
    }

    public EnemyDisplay GetRandomEnemy()
    {
        int randomIndex = Random.Range(0, enemies.Count);
        return enemies[randomIndex];
    }

    public void ProcessEnemyEffects()
    {
        foreach (EnemyDisplay enemy in enemies)
        {
            enemy.ProcessTurnEffects();
        }
    }

    public bool AllEnemiesDefeated()
    {
        return enemies.Count == 0;
    }

    public void RemoveEnemy(EnemyDisplay enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
            Destroy(enemy.gameObject);
        }
    }

    public void ClearEnemies()
    {
        foreach (EnemyDisplay enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
        enemies.Clear();
    }
}
