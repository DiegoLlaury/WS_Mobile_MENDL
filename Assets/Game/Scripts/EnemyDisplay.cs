using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WS_DiegoCo_Enemy;
using System;
using WS_DiegoCo;
using System.Collections.Generic;
using System.Linq;
using WS_DiegoCo_Event;
using UnityEngine.InputSystem;
using System.Collections;
using WS_DiegoCo_Middle;

public class EnemyDisplay : MonoBehaviour, IStatusReceiver
{
    private EventBattle.EventType currentEventType;
    public Enemy enemyData;

    public Image enemyIdleImage;
    public Image enemyAttackImage;
    public Image enemyDamagedImage;
    public Image intentionImage;
    public TMP_Text nameText;
    public TMP_Text healthText;
    public TMP_Text defenseText;
    public TMP_Text perceptionText;
    public TMP_Text discretionText;
    public TMP_Text damageText;


    public GameObject combatStatPanel;
    public GameObject infiltrationStatPanel;
    public GameObject investigationStatPanel;

    public float shakeIntensity = 10f;
    public float shakeDuration = 0.5f;
    public Color damageColor = new Color(1f, 0f, 0f); // Rouge semi-transparent
    public int perceptionLimit = 10;

    private Vector3 originalPosition;
    private Color originalColor;

    private bool isNextAttackHeal;

    private PlayerEvent player;
    private Dictionary<StatusEffect.StatusType, (int value, int turnsRemaining)> activeEffects = new Dictionary<StatusEffect.StatusType, (int, int)>();

    public Transform HealthTransform;
    public Transform DefenseTransform;
    public Transform AttackTransform;
    public Transform PerceptionTransform;
    public Transform DiscretionTransform;


    private void Awake()
    {
        originalPosition = enemyIdleImage.transform.localPosition;
        originalColor = enemyIdleImage.color;
    }

    public void Initialize(Enemy data, EventBattle.EventType eventType)
    {
        enemyData = data;
        currentEventType = eventType;
        player = FindAnyObjectByType<PlayerEvent>();

        enemyData.health = enemyData.maxHealth;
        enemyData.damage = enemyData.maxDamage;
        enemyData.discretion = enemyData.maxDiscretion;
        enemyData.perception = enemyData.maxPerception;
        enemyData.defense = enemyData.maxDefense;
        enemyIdleImage.sprite = enemyData.enemyIdleImage;
        isNextAttackHeal = false;
        UpdateEnemyDisplay();
    }

    public void UpdateEnemyDisplay()
    {
        nameText.text = enemyData.enemyName;
        healthText.text = enemyData.health.ToString();
        defenseText.text = enemyData.defense.ToString();
        damageText.text = enemyData.damage.ToString();
        perceptionText.text = enemyData.perception.ToString();
        discretionText.text = enemyData.discretion.ToString();


        combatStatPanel.SetActive(currentEventType == EventBattle.EventType.Combat);
        infiltrationStatPanel.SetActive(currentEventType == EventBattle.EventType.Infiltration);
        investigationStatPanel.SetActive(currentEventType == EventBattle.EventType.Enquete);
    }

    public void SetNextAttackHeal()
    {
        isNextAttackHeal = true;
        Debug.Log("Next attack will heal the player.");
    }

    public void TakeDamage(int damage, bool ignoreShield = false)
    {
        Debug.Log(isNextAttackHeal);
        if (!ignoreShield && enemyData.defense > 0)
        {
            int absorbed = Mathf.Min(damage, enemyData.defense);
            enemyData.defense -= absorbed;
            damage -= absorbed;
            Debug.Log($"Enemy {enemyData.enemyName} shield absorbed {absorbed} damage. Remaining shield: {enemyData.defense}");
            StartCoroutine(ScaleAnimation(DefenseTransform, 1.5f, 0.5f, 0f));
        }

        if (damage > 0)
        {
            enemyData.health -= damage;
            Debug.Log($"Enemy {enemyData.enemyName} took {damage} damage. Remaining health: {enemyData.health}");
            StartCoroutine(ScaleAnimation(HealthTransform, 1.5f, 0.75f, 0f));
        }

        if (isNextAttackHeal == true)
        {
            player.GainHealth(damage);
            isNextAttackHeal = false;
        }

        if(GameManager.currentEvent.eventType == EventBattle.EventType.Infiltration)
        {
            GameManager.currentEvent.currentTurn = 2;
        }
        
        if(enemyData.health <= 0)
        {
            Debug.Log($"Enemy {enemyData.enemyName} defeated!");
            EnemyManager.Instance.RemoveEnemy(this); // Suppression propre de l'ennemi
            Destroy(gameObject); // D�truit l'objet de l'ennemi dans la sc�ne
            return; // Emp�che l'ex�cution du reste de la m�thode
        }
        
        if(GameManager.currentEvent.eventType == EventBattle.EventType.Enquete)
        {
            Debug.Log("You failed !");
        }

        StartCoroutine(DamageAnimation());
        
        UpdateEnemyDisplay();
        player.Attack();
    }

    private IEnumerator DamageAnimation()
    {
        // 1. Changement de couleur vers le rouge
        enemyIdleImage.color = damageColor;
        enemyIdleImage.sprite = enemyData.enemyDamagedImage;

        // 2. Tremblement
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            Vector3 randomOffset = UnityEngine.Random.insideUnitSphere * shakeIntensity;
            randomOffset.z = 0;  // �vite de bouger sur l'axe Z si en 2D

            enemyIdleImage.transform.localPosition = originalPosition + randomOffset;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 3. Remettre en place l'image et la couleur d'origine
        enemyIdleImage.transform.localPosition = originalPosition;
        enemyIdleImage.color = originalColor;
        enemyIdleImage.sprite = enemyData.enemyIdleImage;
    }

    public IEnumerator DeathAnimation()
    {
        // 1. Changement de couleur vers le rouge et tremblement
        enemyIdleImage.color = damageColor;
        enemyIdleImage.sprite = enemyData.enemyDamagedImage;

        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            Vector3 randomOffset = UnityEngine.Random.insideUnitSphere * shakeIntensity;
            randomOffset.z = 0; // �vite de bouger sur l'axe Z si en 2D

            enemyIdleImage.transform.localPosition = originalPosition + randomOffset;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 2. Remettre l'ennemi � sa position initiale
        enemyIdleImage.transform.localPosition = originalPosition;

        // 3. Commencer le Fade Out
        float fadeDuration = 1.5f; // Temps pour dispara�tre
        float fadeElapsed = 0f;
        Color currentColor = enemyIdleImage.color;

        while (fadeElapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, fadeElapsed / fadeDuration);
            enemyIdleImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);

            fadeElapsed += Time.deltaTime;
            yield return null;
        }

        // 4. Assurez-vous que l'ennemi est compl�tement invisible
        enemyIdleImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0f);
        enemyIdleImage.gameObject.SetActive(false); // D�sactive l'objet une fois l'animation termin�e
    }

    public void GainShield(int amount)
    {
        enemyData.defense += amount;
        StartCoroutine(ScaleAnimation(DefenseTransform, 1.5f, 0.5f, 0f));
        UpdateEnemyDisplay();
        Debug.Log($"Enemy {enemyData.enemyName} gained {amount} shield. Current shield: {enemyData.defense}");
    }

    public void ModifyStat(Card.StatType stat, int amount)
    {
        switch (stat)
        {
            case Card.StatType.health:
                enemyData.health += amount;
                StartCoroutine(ScaleAnimation(HealthTransform, 1.5f, 0.5f, 0f));
                break;
            case Card.StatType.defense:
                enemyData.defense += amount;
                StartCoroutine(ScaleAnimation(DefenseTransform, 1.5f, 0.5f, 0f));
                break;
            case Card.StatType.damage:
                enemyData.damage += amount;
                StartCoroutine(ScaleAnimation(AttackTransform, 1.5f, 0.5f, 0f));
                break;
            case Card.StatType.discretion:
                enemyData.discretion += amount;
                StartCoroutine(ScaleAnimation(DiscretionTransform, 1.5f, 0.5f, 0f));
                break;
            case Card.StatType.perception:
                enemyData.perception += amount;
                StartCoroutine(ScaleAnimation(PerceptionTransform, 1.5f, 0.5f, 0f));
                break;
        }
        UpdateEnemyDisplay();
    }

    public void ReducePerception(int value)
    {
        enemyData.perception = Mathf.Clamp(enemyData.perception - value, 0, enemyData.maxPerception);
        StartCoroutine(ScaleAnimation(PerceptionTransform, 1.5f, 0.5f, 0f));
        UpdateEnemyDisplay();
    }

    public void ReduceInfiltration(int value)
    {
        enemyData.discretion = Mathf.Clamp(enemyData.discretion - value, 0, enemyData.maxDiscretion);
        StartCoroutine(ScaleAnimation(DiscretionTransform, 1.5f, 0.5f, 0f));
        UpdateEnemyDisplay();
    }

    public void ApplyStatus(StatusEffect.StatusType statusType, int value, int duration, EnemyDisplay enemy)
    {
        if (activeEffects.ContainsKey(statusType))
        {
            enemyData = enemy.enemyData;
            // Refresh duration or stack values
            activeEffects[statusType] = (activeEffects[statusType].value + value, duration);

            
        }
        else
        {
            activeEffects.Add(statusType, (value, duration));
        }

        switch (statusType)
        {
            case StatusEffect.StatusType.Weakness:
                enemyData.damage -= value;
                Debug.Log($"Enemy {enemyData.enemyName} is weakened!");
                break;

            case StatusEffect.StatusType.Strength:
                enemyData.damage += value;
                Debug.Log($"Enemy {enemyData.enemyName} strength increased by {value}.");
                break;
        }

        UpdateEnemyDisplay();
        Debug.Log($"Enemy {enemyData.enemyName} applied status {statusType} with value {value} for {duration} turns.");
    }

    public void ProcessTurnEffects(bool endStatus)
    {
        List<StatusEffect.StatusType> toRemove = new List<StatusEffect.StatusType>();

        foreach (var key in activeEffects.Keys.ToList())
        {
            var effect = activeEffects[key];


            
                switch (key)
                {
                    case StatusEffect.StatusType.Bleeding:
                        TakeDamage(effect.value, ignoreShield: true);
                        Debug.Log($"Enemy {enemyData.enemyName} suffered {effect.value} bleeding damage.");
                        break;
                }
            


 
                // Reduce duration
                int newTurns = effect.turnsRemaining - 1;
                if (newTurns <= 0)
                {
                    toRemove.Add(key);

                        switch (key)
                        {
                            case StatusEffect.StatusType.Weakness:
                                enemyData.damage += effect.value;
                                Debug.Log($"Enemy {enemyData.enemyName} is weakened!");
                                break;

                            case StatusEffect.StatusType.Strength:
                                enemyData.damage -= effect.value;
                                Debug.Log($"Enemy {enemyData.enemyName} strength increased by {effect.value}.");
                                break;
                        }
          
                }          
                else
                {
                    activeEffects[key] = (effect.value, newTurns);
                }

            UpdateEnemyDisplay();
        }

        // Remove expired effects
        foreach (var status in toRemove)
        {
            activeEffects.Remove(status);
            Debug.Log($"Enemy {enemyData.enemyName} status {status} expired.");
        }
    }

    public void UpdateIntentionImage(int action)
    {
        if (player.cardData.perception >= perceptionLimit)
        {
            switch (currentEventType)
            {
                case EventBattle.EventType.Combat:
                    switch (action)
                    {
                        case 0: intentionImage.sprite = enemyData.attackImage; break;  // Attaque
                        case 1: intentionImage.sprite = enemyData.defenseImage; break;  // D�fense
                        case 2: intentionImage.sprite = enemyData.buffImage; break;    // Buff
                        case 3: intentionImage.sprite = enemyData.debuffImage; break;  // Debuff
                    }
                    break;

                case EventBattle.EventType.Infiltration:
                    switch (action)
                    {
                        case 0: intentionImage.sprite = enemyData.buffImage; break;  // Gain perception
                        case 1: intentionImage.sprite = enemyData.rondeImage; break;      // Test de ronde
                        case 2: intentionImage.sprite = enemyData.debuffImage; break;      // Debuff discretion
                    }
                    break;

                case EventBattle.EventType.Enquete:
                    switch (action)
                    {
                        case 0: intentionImage.sprite = enemyData.buffImage; break;  // Gain discr�tion
                        case 1: intentionImage.sprite = enemyData.debuffImage; break;      // Debuff perception
                    }
                    break;
            }
            StartCoroutine(ShowIntentionAnimation());
        }
        else
        {
            intentionImage.sprite = enemyData.questionImage;
        }
       
    }

    private IEnumerator ShowIntentionAnimation()
    {
        intentionImage.transform.localScale = Vector3.zero;
        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            intentionImage.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, elapsed / duration);
            yield return null;
        }
    }

    private IEnumerator ScaleAnimation(Transform target, float scaleMultiplier, float duration, float delay)
    {
        Debug.Log("TEST");
        Vector3 originalScale = target.localScale;
        Vector3 targetScale = originalScale * scaleMultiplier;

        yield return new WaitForSeconds(delay);

        float elapsedTime = 0f;
        while (elapsedTime < duration / 2)
        {
            float t = elapsedTime / (duration / 2);
            t = t * t * (3f - 2f * t); // Smoothstep easing
            target.localScale = Vector3.Lerp(originalScale, targetScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < duration / 2)
        {
            float t = elapsedTime / (duration / 2);
            t = t * t * (3f - 2f * t); // Smoothstep easing
            target.localScale = Vector3.Lerp(targetScale, originalScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        target.localScale = originalScale;
    }
}
