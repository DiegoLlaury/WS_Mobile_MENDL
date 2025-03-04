using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

using WS_DiegoCo;

public class CardMovement : MonoBehaviour, IDragHandler, IPointerDownHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 originalLocalPointerPosition;
    private Vector3 originalPanelLocalPosition;
    private Vector3 originalScale;
    public int currentState = 0;
    private Quaternion originalRotation;
    private Vector3 originalPosition;

    [SerializeField] private float selectScale = 1.1f;
    [SerializeField] private GameObject glowEffect;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.localPosition;
        originalRotation = rectTransform.localRotation;
    }

    void Update()
    {
        switch (currentState)
        {
            case 1:
                HandleHoverState();
                break;

            case 2:
                HandleDragState();
                if (Input.touchCount == 0) // Check if touch is released
                {
                    TransitionToState0();
                }
                break;

            case 3:
                HandlePlayState();
                if (Input.touchCount == 0) // Check if touch is released
                {
                    TransitionToState0();
                }
                break;
        }
    }

    private void TransitionToState0()
    {
        currentState = 0;
        rectTransform.localScale = originalScale;
        rectTransform.localRotation = originalRotation;
        rectTransform.localPosition = originalPosition;
        glowEffect.SetActive(false);
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
            Vector2 localPointerPosition;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.GetComponent<RectTransform>(),
                    eventData.position,
                    eventData.pressEventCamera,
                    out localPointerPosition))
            {
                rectTransform.position = eventData.position; // Use eventData.position instead of Input.mousePosition
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.pointerEnter != null) // Ensure something is under the pointer
        {
            // Get the topmost GameObject in case we're hovering over a child object
            GameObject targetObject = eventData.pointerEnter.transform.parent.parent.parent.gameObject;

            // Try to get the EnemyDisplay script from the root object
            EnemyDisplay enemy = targetObject.GetComponent<EnemyDisplay>();

            if (enemy != null) // If an enemy was found
            {
                Debug.Log("Card dropped on enemy!");
                // enemy.TakeDamage(cardData.attackPower); // Apply damage from card
            }
            else
            {
                Debug.Log($"Pointer entered: {eventData.pointerEnter.transform.root.name}");
            }
        }
    }

    //public void OnEndDrag(PointerEventData eventData)
    //{
    //    canvasGroup.alpha = 1f;
    //    canvasGroup.blocksRaycasts = true;

    //    if (!eventData.pointerEnter || !eventData.pointerEnter.GetComponent<EnemyDropZone>())
    //    {
    //        ResetCard(); // Si on ne drop pas sur un ennemi, la carte revient en main
    //    }
    //}

    private void ResetCard()
    {
        Debug.Log("Test");
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

    private void HandleDragState()
    {
        rectTransform.localRotation = Quaternion.identity;
    }

    private void HandlePlayState()
    {
      
    }
}