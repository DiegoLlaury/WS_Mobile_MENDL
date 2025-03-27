using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WS_DiegoCo_Middle;
using WS_DiegoCo_Event;
using WS_DiegoCo_Enemy;
using WS_DiegoCo;
using UnityEngine.Rendering;
using NUnit.Framework.Internal.Commands;
using static StatusEffect;
using static UnityEngine.Rendering.DebugUI;


public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    public List<EnemyDisplay> enemies = new List<EnemyDisplay>();
    private EventBattle eventBattle;

    public Transform enemySpawnPoint;
    public GameObject enemyPrefab;

    [SerializeField] private int defenseEnemy = 2;
    [SerializeField] private int debuffAttack = 1;
    [SerializeField] private int buffAttack = 1;
    [SerializeField] private int buffPerception = 3;
    [SerializeField] private int maxbuffPerception = 3;
    [SerializeField] private int debuffPerception = -2;
    [SerializeField] private int buffDiscretion = 3;
    [SerializeField] private int maxbuffDiscretion = 3;

    [SerializeField] private int DebuffDiscretion = -3;

    public float fanSpread = 5f;
    public float enemiesSpacing = 100f;
    public float verticalSpacing = 10f;
    public float moveDuration = 0.5f;

    public List<GameObject> enemiesInBoard = new List<GameObject>();
    private List<int> enemyActions = new List<int>();
    private int actionEnemy;

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

    private void Update()
    {
        UpdateEnemiesPosition();
    }

    public void StartCombat(EventBattle combatEvent)
    {
        buffPerception = maxbuffPerception;
        buffDiscretion = maxbuffDiscretion;

        switch (GameManager.currentEvent.eventDifficulty)
        {
            case EventBattle.EventDifficulty.Facile:
                break;

            case EventBattle.EventDifficulty.Moyen:
                buffDiscretion += 1;
                buffPerception += 1;
                break;

            case EventBattle.EventDifficulty.Difficile:
                buffDiscretion += 1;
                buffPerception += 1;
                break;
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

        GameObject newEnemyObj = Instantiate(enemyPrefab, enemySpawnPoint.position, Quaternion.identity, enemySpawnPoint);

        EnemyDisplay enemyDisplay = newEnemyObj.GetComponent<EnemyDisplay>();
        if (enemyDisplay != null)
        {
            enemyDisplay.Initialize(enemyData, GameManager.currentEvent.eventType);
            enemies.Add(enemyDisplay);
        }
        else
        {
            Debug.LogError("Enemy prefab is missing EnemyDisplay component!");
        }

        enemiesInBoard.Add(newEnemyObj);
        UpdateEnemiesPosition();
    }

    private void UpdateEnemiesPosition()
    {
        int enemyCount = enemiesInBoard.Count;
        if (enemyCount == 0) return;

        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 targetPos = CalculateCardPosition(i, enemyCount);
            Quaternion targetRot = CalculateCardRotation(i, enemyCount);
            StartCoroutine(AnimateCardMovement(enemiesInBoard[i], targetPos, targetRot));
        }
    }

    private IEnumerator AnimateCardMovement(GameObject card, Vector3 targetPos, Quaternion targetRot)
    {
        if (card == null)
        {
            yield break;
        }

        float elapsedTime = 0f;
        Vector3 startPos = card.transform.position;
        Quaternion startRot = card.transform.rotation;

        while (elapsedTime < moveDuration)
        {
            if (card == null)  // Vérifie à chaque frame
            {
                yield break;
            }

            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;

            card.transform.position = Vector3.Lerp(startPos, targetPos, t);
            card.transform.rotation = Quaternion.Lerp(startRot, targetRot, t);

            yield return null;
        }
        if (card != null)
        {
            card.transform.position = targetPos;
            card.transform.rotation = targetRot;
        } // Final check before setting position{           
    }

    private Vector3 CalculateCardPosition(int index, int totalCards)
    {
        float horizontalOffset = enemiesSpacing * (index - (totalCards - 1) / 2f);
        float normalizedPosition = (totalCards > 1) ? (2f * index / (totalCards - 1) - 1f) : 0f;
        float verticalOffset = verticalSpacing * (1 - normalizedPosition * normalizedPosition);

        return enemySpawnPoint.position + new Vector3(horizontalOffset, verticalOffset, 0f);
    }

    private Quaternion CalculateCardRotation(int index, int totalCards)
    {
        float rotationAngle = fanSpread * (index - (totalCards - 1) / 2f);
        return Quaternion.Euler(0f, 0f, rotationAngle);
    }

    public void GenerateEnemyActions(EventBattle.EventDifficulty difficulty, EventBattle.EventType eventType)
    {
        enemyActions.Clear();  // Reset les actions précédentes

        foreach (var enemy in enemies)
        {
            int action = GetActionByProbability(difficulty, eventType);
            enemyActions.Add(action);
            enemy.UpdateIntentionImage(action);
            Debug.Log($"{enemy.enemyData.enemyName} va faire l'action : {action}");
        }
    }

    public void UpdateEnemyIntentions()
    {
       for (int i = 0; i < enemies.Count; i++)
       {
           EnemyDisplay enemy = enemies[i];
           int actionType = enemyActions[i];

           enemy.UpdateIntentionImage(actionType);
       }   
    }

    public IEnumerator EnemyTurn(EventBattle eventBattle)
    {
        Debug.Log("Enemy Turn Start");
        List<EnemyDisplay> enemiesToRemove = new List<EnemyDisplay>();

        for (int i = 0; i < enemies.Count; i++)
        {
            EnemyDisplay enemy = enemies[i];

            enemy.ProcessTurnEffects(false);
            
            if (enemy.enemyData.health <= 0)
            {
                enemiesToRemove.Add(enemy);
                continue;
            }

            // Utilise l'action générée à l'avance
            int actionType = enemyActions[i];

            switch (eventBattle.eventType)
            {
                case EventBattle.EventType.Combat:
                    PerformActionBattle(enemy, eventBattle.eventDifficulty, actionType);
                    break;

                case EventBattle.EventType.Infiltration:
                    PerformActionInfiltration(enemy, eventBattle.eventDifficulty, actionType);
                    break;

                case EventBattle.EventType.Enquete:
                    PerformActionInvestigation(enemy, eventBattle.eventDifficulty, actionType);
                    break;
            }

            enemy.enemyIdleImage.sprite = enemy.enemyData.enemyAttackImage;
            yield return new WaitForSeconds(0.75f);
            enemy.enemyIdleImage.sprite = enemy.enemyData.enemyIdleImage;
            yield return new WaitForSeconds(0.5f);
        }

        foreach (EnemyDisplay enemy in enemiesToRemove)
        {
            RemoveEnemy(enemy);
        }

        BattleManager.Instance.EndEnemyTurn();
    }

    private void PerformActionBattle(EnemyDisplay enemy, EventBattle.EventDifficulty difficulty, int actionCombatType)
    {

        switch (actionCombatType)
        {
            case 0: // Attack
                BattleManager.Instance.player.TakeDamage(enemy.enemyData.damage);
                enemy.attackSound.Play();
                Debug.Log($"{enemy.enemyData.enemyName} attacks for {enemy.enemyData.damage} damage!");
                break;

            case 1: // Defense
                enemy.ModifyStat(Card.StatType.defense, defenseEnemy);
                Debug.Log($"{enemy.enemyData.enemyName} defense for {enemy.enemyData.defense} defense!");
                break;

            case 2: // Buff
                enemy.ApplyStatus(StatusEffect.StatusType.Strength, buffAttack, 1, enemy);
                Debug.Log($"{enemy.enemyData.enemyName} buffs its damage!");
                break;

            case 3: // Debuff
                BattleManager.Instance.player.ApplyStatus(StatusEffect.StatusType.Weakness, debuffAttack, 1, enemy);
                Debug.Log($"{enemy.enemyData.enemyName} weakens the player!");
                break;
        }

        enemy.UpdateEnemyDisplay();
    }

    private void PerformActionInfiltration(EnemyDisplay enemy, EventBattle.EventDifficulty difficulty, int actionInfiltrationType)
    {
        switch (actionInfiltrationType)
        {
            case 0:
                enemy.ModifyStat(Card.StatType.perception, buffPerception);
                Debug.Log($"{enemy.enemyData.enemyName} gain perception");
                break;

            case 1:
                if (GameManager.currentEvent.currentTurn <= GameManager.currentEvent.numberTurn - 3)
                {
                    BattleManager.Instance.player.RondeTest(enemy);
                    Debug.Log($"{enemy.enemyData.enemyName} do a check");
                }
                else
                {
                    PerformActionInfiltration(enemy, difficulty, Random.Range(0, 1) * 2);
                }
                    break;

            case 2:
                BattleManager.Instance.player.ApplyDebuff(Card.StatType.discretion, DebuffDiscretion);
                Debug.Log($"{enemy.enemyData.enemyName} debuffs yours discretion!");
                break;
        }
    }

    private void PerformActionInvestigation(EnemyDisplay enemy, EventBattle.EventDifficulty difficulty, int actionInvestigationType)
    {
        switch (actionInvestigationType)
        {
            case 0:
                enemy.ModifyStat(Card.StatType.discretion, buffDiscretion);
                Debug.Log($"{enemy.enemyData.enemyName} gain discretion");
                break;

            case 1:
                BattleManager.Instance.player.ApplyDebuff(Card.StatType.perception, debuffPerception);
                Debug.Log($"{enemy.enemyData.enemyName} debuff your perception!");
                break;
        }
    }

    public EnemyDisplay GetRandomEnemy()
    {
        if (enemies.Count == 0)
        {
            Debug.LogWarning("GetRandomEnemy() : Aucun ennemi disponible !");
            return null; // Retourne null pour éviter l'erreur
        }
        int randomIndex = Random.Range(0, enemies.Count);
        return enemies[randomIndex];
    }

    public void ProcessEnemyEffects()
    {
        foreach (EnemyDisplay enemy in enemies)
        {
            //enemy.ProcessTurnEffects();   
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
            StartCoroutine(DeathAnimationCoroutine(enemy));
            enemies.Remove(enemy);
        }
    }

    private IEnumerator DeathAnimationCoroutine(EnemyDisplay enemy)
    {
        if (enemy != null)
        {
            // Lancer l'animation de mort
            yield return StartCoroutine(enemy.DeathAnimation());

            // Vérifie à nouveau que l'ennemi n'a pas été détruit avant de tenter de le détruire
            if (enemy != null)
            {
                // Détruire l'ennemi après la fin de l'animation
                Destroy(enemy.gameObject);
            }
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

    public int GetActionByProbability(EventBattle.EventDifficulty difficulty, EventBattle.EventType eventType)
    {
        int[] probabilities;

        switch (eventType)
        {
            case EventBattle.EventType.Combat:
                probabilities = difficulty switch
                {
                    EventBattle.EventDifficulty.Facile => new int[] { 70, 30 },
                    EventBattle.EventDifficulty.Moyen => new int[] { 65, 25, 10 },
                    EventBattle.EventDifficulty.Difficile => new int[] { 60, 20, 10, 10 },
                    _ => new int[] { 50, 30, 15, 5 }
                };
                break;

            case EventBattle.EventType.Infiltration:
                probabilities = difficulty switch
                {
                    EventBattle.EventDifficulty.Facile => new int[] { 65, 35 },
                    EventBattle.EventDifficulty.Moyen => new int[] { 60, 20, 20 },
                    EventBattle.EventDifficulty.Difficile => new int[] { 70, 15, 15 },
                    _ => new int[] { 50, 30, 20 }
                } ;
                break;

            case EventBattle.EventType.Enquete:
                probabilities = difficulty switch
                {
                    EventBattle.EventDifficulty.Facile => new int[] { 100 },
                    EventBattle.EventDifficulty.Moyen => new int[] { 70, 30 },
                    EventBattle.EventDifficulty.Difficile => new int[] { 60, 40 },
                    _ => new int[] { 70, 30 }
                };
                break;

            default:
                probabilities = new int[] { 100 };
                break;
        }

        int roll = Random.Range(0, 100);
        int cumulative = 0;

        for (int i = 0; i < probabilities.Length; i++)
        {
            cumulative += probabilities[i];
            if (roll < cumulative)
            {
                Debug.Log(i);
                return i;
            }
        }

        return 0;
    }

}
