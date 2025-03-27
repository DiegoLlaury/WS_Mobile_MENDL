using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using WS_DiegoCo_Middle;
using WS_DiegoCo;
using UnityEngine.Rendering;
using System.Linq;
using WS_DiegoCo_Enemy;
using static UnityEngine.GraphicsBuffer;
using WS_DiegoCo_Event;

public class PlayerEvent : MonoBehaviour, IStatusReceiver
{
    public CardMiddle cardData;

    [Header("UI Elements")]
    public TMP_Text energyText, defenseText, healthText, strenghtText, discretionText, perceptionText, currentTurnText;
    public TMP_Text regenNumberTurn, strengthNumberTurn, weaknessNumberTurn;
    public GameObject widgetLost, widgetWin;
    public Image backgroundImage, characterImage;
    public Image regenImage, strengthImage, weaknessImage;

    [Header("Stats")]
    [SerializeField] private int healthRatio = 5;
    [SerializeField] private int baseHealth = 10;
    public int maxEnergy = 3;
    private int currentEnergy;
    public int currentDefense;
    private bool discretionBoost = false;

    public TMP_Text conditionWinText;

    [Header("Status Management")]
    private Dictionary<StatusEffect.StatusType, (int value, int duration)> activeEffects = new Dictionary<StatusEffect.StatusType, (int, int)>();
    private List<System.Action> temporaryEffects = new List<System.Action>();

    public Transform PlayerHealth;
    public Transform PlayerDefense;
    public Transform PlayerAttack;
    public Transform PlayerDiscretion;
    public Transform PlayerPerception;

    public Transform EnergyFrame;

    private int discretionStrengthIncrease;

    public AudioSource damageSound;
    public AudioSource shieldHitSound;
    public AudioSource healSound;
    public AudioSource buffSound;
    public AudioSource debuffSound;
    public AudioSource shieldGainSound;
    public AudioSource energySound;
    public AudioSource errorSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cardData = GameManager.selectedCard;
        currentEnergy = maxEnergy;

        // Recalcule les PV max proprement sans empilement
        cardData.maxHealth = baseHealth + cardData.heart * healthRatio;
        cardData.health = cardData.maxHealth;

        // Remet les autres stats
        cardData.strenght = cardData.maxStrenght;
        cardData.discretion = cardData.maxDiscretion;
        cardData.perception = cardData.maxPerception;
        cardData.defense = currentDefense;

        // Met à jour l'interface
        backgroundImage.sprite = GameManager.currentEvent.background;
        GameManager.currentEvent.currentTurn = GameManager.currentEvent.numberTurn;
        characterImage.sprite = cardData.cardImage;
        UpdatePlayerEvent();
    }
    private void UpdatePlayerEvent()
    {
        healthText.text = cardData.health.ToString();
        strenghtText.text = cardData.strenght.ToString();
        discretionText.text = cardData.discretion.ToString();
        perceptionText.text = cardData.perception.ToString();
        energyText.text = currentEnergy.ToString();
        defenseText.text = currentDefense.ToString();
        currentTurnText.text = GameManager.currentEvent.currentTurn.ToString();
        
    }

    public void TurnChange()
    {
        // Applique les effets temporaires et les vide

        GameManager.currentEvent.currentTurn--;
        ProcessTurnEffects();
        foreach (var effect in temporaryEffects)
        {
            effect.Invoke();
        }
        temporaryEffects.Clear();
        UpdatePlayerEvent();
    }

    // ==================== GESTION DES ACTIONS ====================

    public void TakeDamage(int damage, bool ignoreShield = false)
    {
        if (!ignoreShield && currentDefense > 0)
        {
            int absorbed = Mathf.Min(damage, currentDefense);
            currentDefense -= absorbed;
            damage -= absorbed;
            shieldHitSound.Play();
            StartCoroutine(ScaleAnimation(PlayerDefense, 1.5f, 0.25f, 0f));
            Debug.Log($"Shield absorbed {absorbed} damage. Remaining shield: {currentDefense}");
        }

        if (damage > 0)
        {
            cardData.health  -= damage;
            damageSound.Play();
            StartCoroutine(ScaleAnimation(PlayerHealth, 1.5f, 0.5f, 0f));
            Debug.Log($"Player took {damage} damage. Remaining health: {cardData.health}");
        }

        UpdatePlayerEvent();

        if (cardData.health <= 0)
        {
            BattleManager.Instance.CheckGameOver();
        }
    }

    public void Attack()
    {
        // Lors d'une attaque, perdre toute la discrétion
        if (cardData.discretion > 0)
        {
            cardData.strenght -= discretionStrengthIncrease;
            discretionStrengthIncrease = 0;
            cardData.discretion = 0;
            discretionBoost = false;
            Debug.Log("Attaque effectuée : Discrétion remise à zéro.");
            UpdatePlayerEvent();
        }
    }

    public void GainHealth(int health)
    {
        if (health > 0)
        {
            healSound.Play();
        }
        else
        {
            damageSound.Play();
        }

        cardData.health = Mathf.Clamp(cardData.health + health, 0, cardData.maxHealth);
        StartCoroutine(ScaleAnimation(PlayerHealth, 1.5f, 0.5f, 0f));
        Debug.Log($"Player gain {health} health. Current health : {cardData.health}");
    }

    public void GainShield(int defense)
    {
        currentDefense += defense;
        shieldGainSound.Play();
        StartCoroutine(ScaleAnimation(PlayerDefense, 1.5f, 0.5f, 0f));
        UpdatePlayerEvent();
        Debug.Log($"Player gain {defense} shield. Current shield : {cardData.defense}");
    }

    public void GainPerception(int perception)
    {
        // Ajout normal de perception, en respectant la limite max
        cardData.perception = Mathf.Clamp(cardData.perception + perception, cardData.maxPerception, 50);
        if (perception > 0)
        {
            buffSound.Play();
        }
        else
        {
            debuffSound.Play();
        }
        StartCoroutine(ScaleAnimation(PlayerPerception, 1.5f, 0.5f, 0f));
        UpdatePlayerEvent();
        EnemyManager.Instance.UpdateEnemyIntentions();
        Debug.Log($"Player gained {perception} perception. Current perception: {cardData.perception}");
    }

    public void GainInfiltration(int infiltration)
    {
        // Ajout normal de discrétion, en respectant la limite max
        cardData.discretion = Mathf.Clamp(cardData.discretion + infiltration, cardData.maxDiscretion, 50);
        if (infiltration > 0)
        {
            buffSound.Play();
        }
        else
        {
            debuffSound.Play();
        }
        StartCoroutine(ScaleAnimation(PlayerDiscretion, 1.5f, 0.5f, 0f));
        UpdatePlayerEvent();
        Debug.Log($"Player gained {infiltration} discretion. Current discretion: {cardData.discretion}");

        // Boost temporaire de Force si la discrétion dépasse 10 et que le boost n'a pas encore été appliqué
        if (cardData.discretion > 10 && !discretionBoost)
        {
            cardData.strenght -= discretionStrengthIncrease;
            discretionStrengthIncrease = cardData.discretion / 3;
            cardData.strenght += discretionStrengthIncrease;
            Debug.Log("Discretion > 10: Temporary Strength boost applied.");
            discretionBoost = true;
        }
    }


    public void GainEnergy(int energy)
    {
        currentEnergy += energy;
        energySound.Play();
        StartCoroutine(ScaleAnimation(EnergyFrame, 1.25f, .5f, 0f));
        UpdatePlayerEvent();
        Debug.Log($"Player gain {energy} energy. Current Energy : {currentEnergy}");
    }

    // ==================== GESTION DES STATUTS ====================

    public void ApplyDebuff(Card.StatType stat, int amount)
    {
           switch (stat)
           {
            case Card.StatType.discretion:
                GainInfiltration(amount);
                break;
            case Card.StatType.perception:
                GainPerception(amount);
                break;
           }

           UpdatePlayerEvent();
           Debug.Log($"Player {stat} changed by {amount}");
    }

    public void ApplyStatus(StatusEffect.StatusType statusType, int value, int duration, EnemyDisplay enemy)
    {
        if (activeEffects.ContainsKey(statusType))
        {
            // Rafraîchit la durée et cumule les valeurs
            activeEffects[statusType] = (activeEffects[statusType].value + value, duration);
        }
        else
        {
            activeEffects[statusType] = (value, duration);
        }

        // Effets immédiats si nécessaire
        switch (statusType)
        {
            case StatusEffect.StatusType.Strength:
                cardData.strenght += value;
                StartCoroutine(ScaleAnimation(PlayerAttack, 1.5f, 0.5f, 0f));
                strengthImage.gameObject.SetActive(true);
                strengthNumberTurn.text = duration.ToString();
                strengthNumberTurn.gameObject.SetActive(true);
                break;

            case StatusEffect.StatusType.Weakness:
                cardData.strenght += value;
                StartCoroutine(ScaleAnimation(PlayerAttack, 1.5f, 0.5f, 0f));
                weaknessImage.gameObject.SetActive(true);
                weaknessNumberTurn.text = duration.ToString();
                weaknessNumberTurn.gameObject.SetActive(true);
                break;

            case StatusEffect.StatusType.Regeneration:
                regenImage.gameObject.SetActive(true);
                regenNumberTurn.text = duration.ToString();
                regenNumberTurn.gameObject.SetActive(true);
                break;
        }

        UpdatePlayerEvent();
        Debug.Log($"Applied {statusType} with value {value} for {duration} turns.");
        CardDisplay.UpdateAllCards(cardData.strenght);
    }

    public void ProcessTurnEffects()
    {
        List<StatusEffect.StatusType> toRemove = new List<StatusEffect.StatusType>();

        foreach (var key in activeEffects.Keys.ToList())
        {
            var effect = activeEffects[key];

            switch (key)
            {
                case StatusEffect.StatusType.Shield:
                    GainShield(effect.value);
                    break;

                case StatusEffect.StatusType.Regeneration:
                    GainHealth(effect.value);
                    break;

                case StatusEffect.StatusType.Bleeding:
                    TakeDamage(effect.value, ignoreShield: true);
                    break;
            }

            // Réduction de la durée de l'effet
            int newTurns = effect.duration - 1;

            // Met à jour les textes du HUD
            switch (key)
            {
                case StatusEffect.StatusType.Strength:
                    strengthNumberTurn.text = newTurns.ToString();
                    break;

                case StatusEffect.StatusType.Weakness:
                    weaknessNumberTurn.text = newTurns.ToString();
                    break;

                case StatusEffect.StatusType.Regeneration:
                    regenNumberTurn.text = newTurns.ToString();
                    break;
            }

            // Supprime les effets expirés
            if (newTurns <= 0)
            {
                toRemove.Add(key);

                // Annule les effets permanents
                if (key == StatusEffect.StatusType.Strength)
                {
                    cardData.strenght -= effect.value;  // Retire uniquement la valeur de cet effet précis
                    StartCoroutine(ScaleAnimation(PlayerAttack, 1.5f, 0.5f, 0f));

                    if (activeEffects.Count(e => e.Key == StatusEffect.StatusType.Strength) == 1)
                    {
                        // Désactive l'icône uniquement quand tous les effets de force sont terminés
                        strengthImage.gameObject.SetActive(false);
                        strengthNumberTurn.gameObject.SetActive(false);
                    }
                }
                else if (key == StatusEffect.StatusType.Weakness)
                {
                    cardData.strenght -= effect.value;
                    StartCoroutine(ScaleAnimation(PlayerAttack, 1.5f, 0.5f, 0f));
                    weaknessImage.gameObject.SetActive(false);
                    weaknessNumberTurn.gameObject.SetActive(false);
                }
                else if (key == StatusEffect.StatusType.Regeneration)
                {
                    regenImage.gameObject.SetActive(false);
                    regenNumberTurn.gameObject.SetActive(false);
                }
            }
            else
            {
                activeEffects[key] = (effect.value, newTurns);
            }
        }

        // Supprime les effets expirés
        foreach (var status in toRemove)
        {
            activeEffects.Remove(status);
            Debug.Log($"Status {status} expired.");
        }


        UpdatePlayerEvent();
        CardDisplay.UpdateAllCards(cardData.strenght);
    }

    public void AddTemporaryEffect(System.Action effect)
    {
        temporaryEffects.Add(effect);
        Debug.Log("Temporary effect added for next turn.");
    }

    public void RondeTest(EnemyDisplay enemy)
    {
        if (cardData.discretion < enemy.enemyData.perception)
        {
            GameManager.WinBattle = false;
            EndBattle();
        }
    }

    public bool CanPlayCard(int cost)
    {
        return currentEnergy >= cost;
    }

    public void UseEnergy( int cost)
    {
        if (currentEnergy >= cost)
        {
            currentEnergy -= cost;
            UpdatePlayerEvent();
            if (cost > 0)
            {
                StartCoroutine(ScaleAnimation(EnergyFrame, 1.25f, 0.5f, 0f));
            }
        }
        else
        {
            Debug.LogWarning("Not enough energy to play this card!");
        }
    }

    public void ResetEnergy()
    {
        currentEnergy = maxEnergy;
        UpdatePlayerEvent();
    }  

    public void EndBattle()
    {
        if (GameManager.currentEvent.tuto)
        {
            GameManager.firstTime = true;
            GameManager.currentEventBattles.Clear();
        }

        if (GameManager.WinBattle == true)
        {
            widgetWin.SetActive(true);
        }
        else
        {
            widgetLost.SetActive(true);
        }
    }

    public void DebugBattleWin()
    {
        GameManager.WinBattle = true;
        EndBattle();
    }

    public void DebugBattleLost()
    {
        GameManager.WinBattle = false;
        EndBattle();
    }

    public void ReturnToMain()
    {
        Debug.Log("Clique");
        GameManager.EndEvent();
    }

    public IEnumerator ScaleAnimation(Transform target, float scaleMultiplier, float duration, float delay)
    {
        Vector3 originalScale = new Vector3(1,1,1);
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

    public IEnumerator NotEnoughEnergy(float duration)
    {
        Debug.Log("uycvcvbzfv");
        Color originalColor = Color.white;
        Color targetColor = Color.red;

        float elapsedTime = 0f;
        while (elapsedTime < duration / 2)
        {
            float t = elapsedTime / (duration / 2);
            t = t * t * (3f - 2f * t); // Smoothstep easing
            EnergyFrame.GetComponent<Image>().color = Color.Lerp(originalColor, targetColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < duration / 2)
        {
            float t = elapsedTime / (duration / 2);
            t = t * t * (3f - 2f * t); // Smoothstep easing
            EnergyFrame.GetComponent<Image>().color = Color.Lerp(targetColor, originalColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        EnergyFrame.GetComponent<Image>().color = originalColor;
    }

    public void UpdateVictoryText()
    {
        EventBattle eventBattle = GameManager.currentEvent;

        switch (eventBattle.eventType)
        {
            case EventBattle.EventType.Combat:
                conditionWinText.text = new string($"Vous devez vaincre tous les ennemis avant {GameManager.currentEvent.numberTurn} tours");
                break;

            case EventBattle.EventType.Enquete:
                conditionWinText.text = new string($"Augmenter votre discretion pour atteindre {GameManager.currentEvent.conditionNumber} de discretion avant la fin du nombre de tour impartie");
                break;

            case EventBattle.EventType.Infiltration:
                conditionWinText.text = new string($"Votre valeur de perception doit être supérieur à la valeur de discretion des ennemis avant la fin du nombre de tour impartie.");
                break;
        }
    }
}