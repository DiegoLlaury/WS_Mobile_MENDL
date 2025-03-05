using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using WS_DiegoCo_Enemy;
using WS_DiegoCo;
using NUnit.Framework;
using System;
using Unity.VisualScripting;
using WS_DiegoCo_Middle;

public class CardMovement : MonoBehaviour, IDragHandler, IPointerDownHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private PlayerEvent player;
    
    private Vector2 originalLocalPointerPosition;
    private Vector3 originalPanelLocalPosition;
    private Vector3 originalScale;
    private Quaternion originalRotation;
    private Vector3 originalPosition;
    
    public int currentState = 0;
    [SerializeField]
    private CardDisplay cardDisplay;
    private Card cardData;
    private DeckManager deckManager;
    private HandManager handManager;
    private EnemyManager enemyManager;


    [SerializeField] private float selectScale = 1.1f;
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

        if (cardDisplay != null)
        {
            cardData = cardDisplay.cardData;
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
            originalRotation = rectTransform.localRotation;
            originalScale = rectTransform.localScale;
            currentState = 1;
            HandleHoverState();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentState == 1)
        {
            TransitionToState0();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.5f;
            canvasGroup.blocksRaycasts = false;
        }

        if (currentState == 1)
        {
            currentState = 2;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                eventData.position,
                eventData.pressEventCamera,
                out originalLocalPointerPosition
            );
            originalPanelLocalPosition = rectTransform.localPosition;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentState == 2)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.GetComponent<RectTransform>(),
                    eventData.position,
                    eventData.pressEventCamera,
                    out Vector2 localPointerPosition))
            {
                rectTransform.position = eventData.position;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        if (eventData.pointerEnter == null)
        {
            Debug.LogWarning("OnEndDrag: No object detected under the pointer.");
            TransitionToState0();
            return;
        }

        GameObject targetObject = eventData.pointerEnter.gameObject;

        // Check if the card is dropped on the DropZone
        if (targetObject.CompareTag("DropZone"))
        {
            HandleCardUsed();
            return;
        }

        if (!player.CanPlayCard(cardData.energy))
        {
            Debug.LogWarning("Not enough energy to play this card!");
            TransitionToState0();
            return;
        }

        // Check if the card is dropped on an enemy
        EnemyDisplay enemy = targetObject.GetComponentInParent<EnemyDisplay>();
        if (enemy != null)
        {
            ApplyCardEffects(enemy);
            player.UseEnergy(cardData.energy);
            if (enemy.enemyData.health <= 0)
            {
                enemyManager.RemoveEnemy(enemy);
            }
            HandleCardUsed();
        }
        else
        {
            Debug.LogWarning("OnEndDrag: Card was not placed on a valid target.");
            TransitionToState0();
        }
    }

    private void ApplyCardEffects(EnemyDisplay enemy)
    {
        if (cardDisplay == null || cardDisplay.cardData == null)
        {
            Debug.LogError("ApplyCardEffects: Card data is missing!");
            return;
        }

        foreach (CardEffect effect in cardDisplay.cardData.effects)
        {
            effect.ApplyEffect(enemy);
        }

        Debug.Log($"Card {cardDisplay.cardData.cardName} applied effects to {enemy.enemyData.enemyName}.");
    }

    private void HandleCardUsed()
    {
        if (deckManager != null && cardData != null)
        {
            deckManager.DiscardCard(cardData);
        }

        if (handManager != null)
        {
            handManager.RemoveCardFromHand(gameObject);
        }

        Destroy(gameObject);
    }

    private void TransitionToState0()
    {
        currentState = 0;
        rectTransform.localScale = originalScale;
        rectTransform.localRotation = originalRotation;
        rectTransform.localPosition = originalPosition;
        glowEffect.SetActive(false);
    }

    private void HandleHoverState()
    {
        glowEffect.SetActive(true);
        rectTransform.localScale = originalScale * selectScale;
    }
}

