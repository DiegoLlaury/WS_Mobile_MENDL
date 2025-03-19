using UnityEngine;
using WS_DiegoCo_Event;
using WS_DiegoCo_Middle;
using UnityEngine.UI;
using TMPro;

public class EventDisplay : MonoBehaviour
{
    public EventBattle eventBattle;
    public GameObject panelInformation;
    public CardMiddle cardMiddle;

    public TMP_Text descriptionText;
    public TMP_Text nameEventText;
    public TMP_Text typeEventText;
    public TMP_Text difficultyText;
    public TMP_Text numberOfTurn;

    public Image cardImage;
    public Image backgroundImage;

    private DeckManagerMacro deckManagerMacro;
    private HandManagerMacro handManagerMacro;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        eventBattle.currentTurn = eventBattle.numberTurn;
        panelInformation.SetActive(false);
        deckManagerMacro = FindAnyObjectByType<DeckManagerMacro>();
        handManagerMacro = FindAnyObjectByType<HandManagerMacro>();
        UpdateEventDisplay();
    }

    private void UpdateEventDisplay()
    {
        nameEventText.text = eventBattle.eventName;
        descriptionText.text = eventBattle.description;
        typeEventText.text = eventBattle.eventType.ToString();
        difficultyText.text = eventBattle.eventDifficulty.ToString();
        numberOfTurn.text = eventBattle.numberTurn.ToString();
        backgroundImage.sprite = eventBattle.background;
        if (eventBattle.assignedOfficer != null)
        {
            cardImage.sprite = eventBattle.assignedOfficer.cardImage;
        }
        else
        {
            cardImage.sprite = null;
        }
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
        handManagerMacro.AddCardToHand(eventBattle.assignedOfficer);
        deckManagerMacro.UnstockCard(eventBattle.assignedOfficer);
        eventBattle.assignedOfficer = null;
        cardImage.sprite = null;
        panelInformation.SetActive(false);
    }

    public void SetPlayer(CardMiddle cardPlayer)
    {
        if (eventBattle.assignedOfficer == null)
        {
            eventBattle.assignedOfficer = cardPlayer;
            cardImage.sprite = cardPlayer.cardImage;
            Debug.Log($"{cardPlayer.cardName} assigné à {eventBattle.eventName}");
        }
        else
        {
            Debug.Log("Un policier est déjà assigné à cet événement");
        }
    }

    public void StartAutoResolution()
    {
        if (eventBattle.assignedOfficer == null)
        {
            Debug.Log("Aucun policier assigné pour la résolution automatique.");
            return;
        }

        GameManager.ResolveEvent(eventBattle);
        UpdateEventDisplay();
    }

    private void StartBattle()
    {
        if (cardMiddle == null)
        {
            Debug.Log("No Card assign to this event");
            return;
        }
        GameManager.StartEvent(cardMiddle, eventBattle);
    }
}
