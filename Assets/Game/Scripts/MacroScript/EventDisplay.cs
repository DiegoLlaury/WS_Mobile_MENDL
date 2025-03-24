using UnityEngine;
using WS_DiegoCo_Event;
using WS_DiegoCo_Middle;
using UnityEngine.UI;
using TMPro;

public class EventDisplay : MonoBehaviour
{
    public EventBattle eventBattle;
    public GameObject panelInformation;
    private CardMiddle cardMiddle;
    public TMP_Text descriptionText;
    public TMP_Text nameEventText;
    public TMP_Text typeEventText;
    public TMP_Text difficultyText;
    public TMP_Text numberOfTurn;

    private DeckManagerMacro deckManagerMacro;
    private HandManagerMacro handManagerMacro;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
<<<<<<< Updated upstream
        eventBattle.currentTurn = eventBattle.numberTurn;
=======
       // currentBattle.isResolved = false;
>>>>>>> Stashed changes
        panelInformation.SetActive(false);
        deckManagerMacro = FindAnyObjectByType<DeckManagerMacro>();
        handManagerMacro = FindAnyObjectByType<HandManagerMacro>();
        UpdateEventDisplay();
    }

    private void UpdateEventDisplay()
    {
<<<<<<< Updated upstream
        nameEventText.text = eventBattle.eventName;
        descriptionText.text = eventBattle.description;
        typeEventText.text = eventBattle.eventType.ToString();
        difficultyText.text = eventBattle.eventDifficulty.ToString();
        numberOfTurn.text = eventBattle.numberTurn.ToString();
=======
        if (buildingImage != null)
        {
            buildingImage.gameObject.SetActive(currentBattle != null);
        }

        if (currentBattle == null)
        {
            return;
        }

        currentBattle.currentTurn = currentBattle.numberTurn;

        if (nameEventText != null) nameEventText.text = currentBattle.eventName;
        if (descriptionText != null) descriptionText.text = currentBattle.description;
        if (typeEventText != null) typeEventText.text = currentBattle.eventType.ToString();
        if (difficultyText != null) difficultyText.text = currentBattle.eventDifficulty.ToString();
        if (numberOfTurn != null) numberOfTurn.text = currentBattle.numberTurn.ToString();
        if (backgroundImage != null) backgroundImage.sprite = currentBattle.background;

        if (cardImage != null)
        {
            cardImage.sprite = cardMiddle != null ? cardMiddle.cardImage : null;
        }
    }
    public void SetEvent(EventBattle eventBattle)
    {
        currentBattle = eventBattle;
        Debug.Log($"EventDisplay lié à {eventBattle.eventName} pour le lieu {buildingPlace}.");
        UpdateEventDisplay();
>>>>>>> Stashed changes
    }

    public void EventCheck()
    {
        if (eventBattle == null)
        {
            Debug.Log("No Event are happening here");
            return;
        }
        panelInformation.SetActive(true);
    }

    public void RemoveCardFromEvent()
    {
        if (cardMiddle == null)
        {
            Debug.Log("No card assign to this event");
            return;
        }
        handManagerMacro.AddCardToHand(cardMiddle);
        deckManagerMacro.UnstockCard(cardMiddle);
        panelInformation.SetActive(false);
        cardMiddle = null;
    }

    public void SetPlayer(CardMiddle cardPlayer)
    {
<<<<<<< Updated upstream
        Debug.Log(cardPlayer.name);
        cardMiddle = cardPlayer;
    }

    private void StartBattle()
=======
        if (cardMiddle == null)
        {
            cardMiddle = cardPlayer;
            cardImage.sprite = cardPlayer.cardImage;
            Debug.Log($"{cardPlayer.cardName} assigné à {currentBattle.eventName}");
            numberOfPlayer++;

            if (EventManager.Instance.AreAllEventsReady())
            {
                Debug.Log("Tous les événements sont prêts. Le combat peut commencer !");
            }
        }
        else
        {
            Debug.Log("Un policier est déjà assigné à cet événement");
        }
    }

    public void StartEvent()
    {
        if (!EventManager.Instance.AreAllEventsReady())
        {
            Debug.Log("Tous les événements n'ont pas encore de carte assignée !");
            return;
        }

        if (cardMiddle != null)
        {
            currentBattle.isResolved = true;
            GameManager.StartEvent(cardMiddle, currentBattle);
        }

    }

    public void StartAutoResolution()
>>>>>>> Stashed changes
    {
        if (cardMiddle == null)
        {
            Debug.Log("No Card assign to this event");
            return;
        }
        GameManager.StartEvent(cardMiddle, eventBattle);
    }
}
