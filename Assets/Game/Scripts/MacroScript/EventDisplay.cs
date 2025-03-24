using UnityEngine;
using WS_DiegoCo_Event;
using WS_DiegoCo_Middle;
using UnityEngine.UI;
using TMPro;
using static WS_DiegoCo_Event.EventBattle;

public class EventDisplay : MonoBehaviour
{
    public static EventDisplay currentActivePanel;

    public EventBattle currentBattle;
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
    private static int numberOfPlayer = 0;

    private DeckManagerMacro deckManagerMacro;
    private HandManagerMacro handManagerMacro;
    public BuildingPlace buildingPlace;

    public enum BuildingPlace
    {
        Police,
        Prison,
        Gare,
        Hopital,
        Villa,
        Casino,
        Bank,
        Cafe,
        Entrepot,
        Diner
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        panelInformation.SetActive(false);
        deckManagerMacro = FindAnyObjectByType<DeckManagerMacro>();
        handManagerMacro = FindAnyObjectByType<HandManagerMacro>();
        UpdateEventDisplay();
    }

    private void UpdateEventDisplay()
    {
        if (buildingImage != null)
        {
            buildingImage.gameObject.SetActive(currentBattle != null);
        }

        if (currentBattle == null)
        {
            return;
        }
        currentBattle.remainingAttempts = currentBattle.MaxAttempts;
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

    public void RefreshDisplay()
    {
        UpdateEventDisplay();
    }

    public void SetEvent(EventBattle eventBattle)
    {
        currentBattle = eventBattle;
        Debug.Log($"EventDisplay lié à {eventBattle.eventName} pour le lieu {buildingPlace}.");

        EventManager.Instance.SetEvents(buildingPlace.ToString(), this);
        UpdateEventDisplay();
    }

    public void EventCheck()
    {
        if (currentBattle == null)
        {
            Debug.Log("No Event are happening here");
            return;
        }
        if (currentActivePanel != null && currentActivePanel != this)
        {
            currentActivePanel.ClosePanel();
        }

        // Active ce panel et le définit comme actif
        UpdateEventDisplay();
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
            return;
        }
        handManagerMacro.AddCardToHand(cardMiddle);
        deckManagerMacro.UnstockCard(cardMiddle);
        currentBattle.affectedCharacter = null;
        cardMiddle = null;
        cardImage.sprite = null;
        panelInformation.SetActive(false);
        numberOfPlayer--;
    }

    public void SetPlayer(CardMiddle cardPlayer)
    {
        if (cardMiddle == null)
        {
            cardMiddle = cardPlayer;
            cardImage.sprite = cardPlayer.cardImage;
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
            currentBattle.isResolved = false;
            
            GameManager.StartEvent(cardMiddle, currentBattle);
        }

    }
}
