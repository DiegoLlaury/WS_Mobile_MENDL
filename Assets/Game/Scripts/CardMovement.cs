using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using WS_DiegoCo;


public class CardMovement : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private PlayerEvent player;
    public CardMovement currentDraggedCard;
    
    private Vector2 originalLocalPointerPosition;
    private Vector3 originalPanelLocalPosition;
    private Vector3 originalScale;
    private Quaternion originalRotation;
    private Vector3 originalPosition;
    private int originalLayer;
    
    public int currentState = 0;
    [SerializeField]
    private CardDisplay cardDisplay;
    private Card cardData;
    private DeckManager deckManager;
    private HandManager handManager;
    private EnemyManager enemyManager;
    private BattleManager battleManager;

    public AudioClip cardPlaceSound; // Son de pose de carte



    [SerializeField] private float selectScale = 1.5f;
    [SerializeField] private GameObject glowEffect;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        cardDisplay = GetComponent<CardDisplay>();

        deckManager = FindAnyObjectByType<DeckManager>();
        handManager = FindAnyObjectByType<HandManager>();
        player = FindAnyObjectByType<PlayerEvent>();
        enemyManager = FindAnyObjectByType<EnemyManager>();
        battleManager = FindAnyObjectByType<BattleManager>();

        if (cardDisplay != null && cardDisplay.cardData != null)
        {
            cardData = cardDisplay.cardData; // Make sure cardData is set properly
        }

        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.localPosition;
        originalRotation = rectTransform.localRotation;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        if (currentState == 0)
        {
            originalPosition = rectTransform.localPosition;
            originalScale = rectTransform.localScale;
            originalRotation = rectTransform.localRotation;

            // Passe au premier plan en changeant l'ordre des cartes (facultatif)
            transform.SetAsLastSibling();

            // Notifie le HandManager
            if (handManager.hoveredCard != gameObject)
            {
                // Réinitialise l'état de la carte précédente, si elle existe
                if (handManager.hoveredCard != null)
                {
                    CardMovement previousCard = handManager.hoveredCard.GetComponent<CardMovement>();
                    if (previousCard != null)
                    {
                        previousCard.TransitionToState0(); // Réinitialise la carte précédente
                    }
                }

                // Mets à jour la carte survolée
                handManager.SetHoveredCard(gameObject);
            }

            // Animation de survol
            HandleHoverState();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentState == 1)
        {
            TransitionToState0();

            handManager.ClearHoveredCard(gameObject);
        }
        else if (currentState == 0)
        {
            TransitionToState0();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        TransitionToState0();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentState == 2) return;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.8f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        originalLayer = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        if (cardDisplay != null && cardDisplay.cardData != null)
        {

        }
        else
        {
            Debug.LogError(" CardDisplay or CardData is NULL on Pointer Down!");
        }

        

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                eventData.position,
                eventData.pressEventCamera,
                out originalLocalPointerPosition
        );
        
        originalPanelLocalPosition = rectTransform.localPosition;
        
        currentState = 2;

        currentDraggedCard = this; // Store the card being dragged
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentState == 2)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out Vector2 localPointerPosition))
            {
                rectTransform.position = eventData.position;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EnemyDisplay enemy = GetEnemyUnderPointer(eventData);
        GameObject dropZone = GetDropZoneUnderPointer(eventData);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }

        gameObject.layer = originalLayer; 

        if (!player.CanPlayCard(cardDisplay.cardData.energy))
        {
            Debug.LogWarning("Not enough energy to play this card!");
            TransitionToState0();
            return;
        }

        if (currentDraggedCard == null)
        {
            Debug.LogWarning("No card detected during drag.");
            TransitionToState0();
            return;
        }

        bool isAttackCard = false;
        bool isSkillCard = false;

        // Explicitly check each DropType in the list
        foreach (var type in cardDisplay.cardData.dropType)
        {
            if (type == Card.DropType.Attack) isAttackCard = true;
            if (type == Card.DropType.Competence) isSkillCard = true;
        }

        if (isAttackCard)
        {
            if (enemy != null)
            {
                ApplyCardEffects(enemy, cardDisplay.cardData, player, deckManager, handManager, battleManager, enemyManager);

                if (enemy.enemyData.health <= 0)
                {
                    enemyManager.RemoveEnemy(enemy);
                }
            }
            else
            {
                Debug.LogWarning("Attack card must target an enemy!");
                canvasGroup.blocksRaycasts = true;
                TransitionToState0();
            }
        }
        else if (isSkillCard)
        {
            
            // Check if dropped on the Skill Zone
            if (dropZone != null && dropZone.CompareTag("DropZone"))
            {
                if (enemy == null)
                {
                    EnemyDisplay randomEnemy = enemyManager.GetRandomEnemy();
                    enemyManager.GetRandomEnemy();
                    ApplyCardEffects(randomEnemy, cardDisplay.cardData, player, deckManager, handManager, battleManager, enemyManager);
                }
                else
                {
                    ApplyCardEffects(enemy, cardDisplay.cardData, player, deckManager, handManager, battleManager, enemyManager);
                }
            }
            else
            {
                Debug.LogWarning($"Skill card {cardDisplay.cardData.cardName} must be played on the Skill Zone.");
                TransitionToState0();
            }
        }
        else
        {
            Debug.LogError($"Card {cardData.cardName} has an unknown DropType!");
            TransitionToState0();
        }

        
    }
    private EnemyDisplay GetEnemyUnderPointer(PointerEventData eventData)
    {
        if (eventData.pointerEnter != null)
        {
            EnemyDisplay enemy = eventData.pointerEnter.GetComponentInParent<EnemyDisplay>();
            if (enemy != null)
            {
                return enemy;
            }
        }

        // Backup detection using raycast
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            EnemyDisplay enemy = hit.collider.GetComponentInParent<EnemyDisplay>();
          
            if (enemy != null)
            {
                return enemy;
            }
        }

        return null; // No enemy found
    }

    private GameObject GetDropZoneUnderPointer(PointerEventData eventData)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = eventData.position
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.CompareTag("DropZone")) // 🔹 Ensure DropZone has correct tag
            {
                return result.gameObject;
            }
        }

        return null; // No drop zone detected
    }

    private void ApplyCardEffects(EnemyDisplay enemy, Card card, PlayerEvent playerEvent, DeckManager deck, HandManager hand, BattleManager battleManager, EnemyManager enemyManager)
    {
        if (enemy == null)
        {
            Debug.LogError("ApplyCardEffects: Enemy is null!");
            return; // Prevent further execution
        }

        
        if (cardPlaceSound != null) // Joue le son AVANT de détruire la carte
        {
                AudioManager.instance.PlaySound(cardPlaceSound);
        }


        bool shouldReturnToHand = false;

        foreach (CardEffect effect in cardDisplay.cardData.effects)
        {
            effect.ApplyEffect(enemy, card, playerEvent, deck, hand, battleManager, enemyManager);

            if (effect.shouldReturnToHand)
            {
                shouldReturnToHand = true;
            }
        }

        battleManager.CheckGameOver();

        if (shouldReturnToHand)
        {
            Debug.Log($"Card {cardDisplay.cardData.cardName} returns to hand due to effect.");
            TransitionToState0();
        }
        else
        {
            player.UseEnergy(cardDisplay.cardData.energy);
            HandleCardUsed(card);
        }
    }

    private void HandleCardUsed(Card card)
    {
        if (deckManager != null && cardData != null)
        {
            deckManager.DiscardCard(card);
        }
        else
        {
            Debug.LogError(deckManager);
            Debug.LogError(cardData);
        }

        if (handManager != null)
        {
            handManager.RemoveCardFromHand(gameObject);
        }

        Destroy(gameObject);
    }

    public void TransitionToState0()
    {
        currentState = 0;
        rectTransform.localScale = originalScale;
        rectTransform.localRotation = originalRotation;
        rectTransform.localPosition = originalPosition;
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;  // Re-enable raycasting
            canvasGroup.interactable = true;
        }

        if (glowEffect != null)
        {
            glowEffect.SetActive(false);
        }

        currentDraggedCard = null;
    }

    private void HandleHoverState()
    {
        glowEffect.SetActive(true);
        rectTransform.localScale = originalScale * selectScale;

        // Lève la carte en avant
        Vector3 hoverOffset = new Vector3(0f, 75f, 0f);
        rectTransform.localPosition += hoverOffset;
    }
}

