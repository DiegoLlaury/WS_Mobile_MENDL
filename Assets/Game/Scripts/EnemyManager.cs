using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WS_DiegoCo_Middle;
using WS_DiegoCo;

public class EnemyManager : MonoBehaviour
{
    public CardMiddle cardMiddle;
    public static EnemyManager Instance;
    public List<EnemyDisplay> enemies = new List<EnemyDisplay>();

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

    public void RegisterEnemy(EnemyDisplay enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }

    public void RemoveEnemy(EnemyDisplay enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
            Destroy(enemy.gameObject);
        }
    }

    public bool AllEnemiesDefeated()
    {
        return enemies.Count == 0;
    }

    public IEnumerator EnemyTurn()
    {
        Debug.Log("Enemy Turn Start");

        foreach (EnemyDisplay enemy in enemies.ToArray()) // Use ToArray to avoid modification errors
        {
            if (enemy.enemyData.health > 0)
            {
                PerformEnemyAction(enemy);
                yield return new WaitForSeconds(2f);
            }
        }

        GameManager.Instance.EndEnemyTurn();
    }

    private void PerformEnemyAction(EnemyDisplay enemy)
    {
        int actionType = Random.Range(0, 3); // 0 = Attack, 1 = Buff, 2 = Debuff

        switch (actionType)
        {
            case 0: // Attack
                GameManager.Instance.player.TakeDamage(enemy.enemyData.damage);
                Debug.Log($"{enemy.enemyData.enemyName} attacks for {enemy.enemyData.damage} damage!");
                break;

            case 1: // Buff
                enemy.ModifyStat(Card.StatType.damage, 1);
                Debug.Log($"{enemy.enemyData.enemyName} buffs its damage!");
                break;

            case 2: // Debuff (weaken player)
                GameManager.Instance.player.ApplyDebuff(Card.StatType.damage, -1);
                Debug.Log($"{enemy.enemyData.enemyName} weakens the player!");
                break;
        }
    }
}
