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
        Debug.Log(cardPlayer.name);
        cardMiddle = cardPlayer;
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
