using UnityEngine;
using WS_DiegoCo_Event;
using WS_DiegoCo_Middle;
using UnityEngine.UI;
using TMPro;
using static WS_DiegoCo_Event.EventBattle;

public class EventDisplay : MonoBehaviour
{
    public static EventDisplay currentActivePanel;

    public EventBattle eventBattle;
    public GameObject panelInformation;
    public CardMiddle cardMiddle;

    public TMP_Text descriptionText;
    public TMP_Text nameEventText;
    public TMP_Text typeEventText;
    public TMP_Text difficultyText;
    public TMP_Text numberOfTurn;

    public Image cardImage;
    public Image buildingImage;
    public Image backgroundImage;

    private DeckManagerMacro deckManagerMacro;
    private HandManagerMacro handManagerMacro;

    public EventPlace eventPlace;

    public enum EventPlace
    {
        Comissariat,
        Prison,
        Gare,
        Hopital,
        Villa,
        Casino,
        Banque,
        Bar,
        Entrepots,
        Diner
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (eventBattle == null)
        {
            buildingImage.gameObject.SetActive(false);
            return;
        }
        else
        {
            buildingImage.gameObject.SetActive(true);
        }
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
        if (cardMiddle != null)
        {
            cardImage.sprite = cardMiddle.cardImage;
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
        if (currentActivePanel != null && currentActivePanel != this)
        {
            currentActivePanel.ClosePanel();
        }

        // Active ce panel et le d�finit comme actif
        panelInformation.SetActive(true);
        currentActivePanel = this;
    }

    public void ClosePanel()
    {
        panelInformation.SetActive(false);
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
        cardMiddle = null;
        cardImage.sprite = null;
        panelInformation.SetActive(false);
    }

    public void SetPlayer(CardMiddle cardPlayer)
    {
        if (cardMiddle == null)
        {
            cardMiddle = cardPlayer;
            cardImage.sprite = cardPlayer.cardImage;
            Debug.Log($"{cardPlayer.cardName} assign� � {eventBattle.eventName}");
        }
        else
        {
            Debug.Log("Un policier est d�j� assign� � cet �v�nement");
        }
    }

    public void StartEvent()
    {
        if (cardMiddle != null)
        {
            GameManager.StartEvent(cardMiddle, eventBattle);
        }

    }

    public void StartAutoResolution()
    {
        if (cardMiddle == null)
        {
            Debug.Log("Aucun policier assign� pour la r�solution automatique.");
            return;
        }

        ResolveEvent(eventBattle);
        UpdateEventDisplay();
    }

    public void ResolveEvent(EventBattle eventBattle)
    {
        if (cardMiddle == null)
        {
            Debug.LogWarning("Aucun policier assign� � cet �v�nement.");
            return;
        }

        // Calcul des chances de succ�s
        int baseSuccessChance = eventBattle.eventDifficulty switch
        {
            EventBattle.EventDifficulty.Facile => 70,
            EventBattle.EventDifficulty.Moyen => 50,
            EventBattle.EventDifficulty.Difficile => 30,
            _ => 50
        };

        int totalChance = baseSuccessChance + cardMiddle.skillLevel;
        bool success = Random.Range(0, 100) < totalChance;

        if (success && cardMiddle.corruption == false)
        {
            Debug.Log($"Succ�s : {eventBattle.eventName}");
            eventBattle.isResolved = true;
            
            EventManager.Instance.ResolveEvent(eventBattle);
            ClosePanel();
        }
        else
        {
            Debug.Log($"�chec : {eventBattle.eventName}");
            eventBattle.remainingAttempts--;

            if (eventBattle.remainingAttempts <= 0)
            {
                Debug.Log($"�v�nement �chou� d�finitivement : {eventBattle.eventName}");
                Debug.Log($"{cardMiddle.cardName} a �t� bless� !");
            }
        }
        UpdateEventDisplay();
    }

    public void DebugTestMacro()
    {
        if (eventBattle == null || cardMiddle == null)
        {
            return; 
        }
        ResolveEvent(eventBattle);
    }
}
