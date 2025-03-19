using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using WS_DiegoCo_Middle;


public class CardMovementMacro : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private PlayerEvent player;
    public CardMovementMacro currentDraggedCard;

    private Vector2 originalLocalPointerPosition;
    private Vector3 originalPanelLocalPosition;
    private Vector3 originalScale;
    private Quaternion originalRotation;
    private Vector3 originalPosition;
    private int originalLayer;

    public int currentState = 0;
    [SerializeField]
    private CardMiddleDisplay cardMiddleDisplay;
    private CardMiddle cardData;
    private DeckManagerMacro deckManagerMacro;
    private HandManagerMacro handManagerMacro;


    [SerializeField] private float selectScale = 1.1f;
    [SerializeField] private GameObject glowEffect;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        cardMiddleDisplay = GetComponent<CardMiddleDisplay>();

        deckManagerMacro = FindAnyObjectByType<DeckManagerMacro>();
        handManagerMacro = FindAnyObjectByType<HandManagerMacro>();
        player = FindAnyObjectByType<PlayerEvent>();

        if (cardMiddleDisplay != null && cardMiddleDisplay.cardData != null)
        {
            cardData = cardMiddleDisplay.cardData; // Make sure cardData is set properly
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

    public void OnPointerUp(PointerEventData eventData)
    {
        if (currentState == 2)
        {
            TransitionToState0();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentState == 2) return;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.5f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        originalLayer = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        if (cardMiddleDisplay != null && cardMiddleDisplay.cardData != null)
        {
            Debug.Log($" Picked Up Card: {cardMiddleDisplay.cardData.cardName} (Instance ID: {cardMiddleDisplay.cardData.GetInstanceID()})");
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
        EventDisplay currenEvent = GetEventUnderPointer(eventData);
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }

        gameObject.layer = originalLayer;

        if (currenEvent != null)
        {
            if (currenEvent.cardMiddle == null)
            {
                currenEvent.SetPlayer(cardData);
                deckManagerMacro.StockCard(cardData);
                handManagerMacro.RemoveCardFromHand(gameObject);
            }
            else
            {
                TransitionToState0();
            }
        }
        else
        {
            Debug.LogWarning("Aucun EventDisplay trouvé sous le pointeur lors du drag.");
        }
    }
    private EventDisplay GetEventUnderPointer(PointerEventData eventData)
    {
        if (eventData.pointerEnter != null)
        {
            EventDisplay currentEvent = eventData.pointerEnter.GetComponentInParent<EventDisplay>();
            if (currentEvent != null)
            {
                return currentEvent;
            }
        }

        // Backup detection using raycast
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            EventDisplay currentEvent = hit.collider.GetComponentInParent<EventDisplay>();

            if (currentEvent != null)
            {
                return currentEvent;
            }
        }

        return null; // No enemy found
    }


    //private void HandleCardUsed()
    //{
    //    if (deckManagerMacro != null && cardData != null)
    //    {
    //        deckManagerMacro.DiscardCard(cardData);
    //    }

    //    if (handManagerMacro != null)
    //    {
    //        handManagerMacro.RemoveCardFromHand(gameObject);
    //    }

    //    Destroy(gameObject);
    //}



    private void TransitionToState0()
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

        glowEffect.SetActive(false);
    }

    private void HandleHoverState()
    {
        glowEffect.SetActive(true);
        rectTransform.localScale = originalScale * selectScale;
    }
}

