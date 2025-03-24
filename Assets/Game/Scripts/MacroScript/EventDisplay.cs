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
        if (currentBattle == null)
        {
            buildingImage.gameObject.SetActive(false);
            Debug.Log("Nique");
            return;
        }
        else
        {
            Debug.Log("la photo");
            buildingImage.gameObject.SetActive(true);
        }
        currentBattle.currentTurn = currentBattle.numberTurn;
        nameEventText.text = currentBattle.eventName;
        descriptionText.text = currentBattle.description;
        typeEventText.text = currentBattle.eventType.ToString();
        difficultyText.text = currentBattle.eventDifficulty.ToString();
        numberOfTurn.text = currentBattle.numberTurn.ToString();
        backgroundImage.sprite = currentBattle.background;
        if (cardMiddle != null)
        {
            cardImage.sprite = cardMiddle.cardImage;
        }
        else
        {
            cardImage.sprite = null;
        }
    }
    public void SetEvent(EventBattle eventBattle)
    {
        currentBattle = eventBattle;
        Debug.Log($"EventDisplay lié à {eventBattle.eventName} pour le lieu {buildingPlace}.");
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
        numberOfPlayer--;
    }

    public void SetPlayer(CardMiddle cardPlayer)
    {
        if (cardMiddle == null)
        {
            cardMiddle = cardPlayer;
            cardImage.sprite = cardPlayer.cardImage;
            Debug.Log(numberOfPlayer);
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
            GameManager.StartEvent(cardMiddle, currentBattle);
        }

    }

    public void StartAutoResolution()
    {
        if (cardMiddle == null)
        {
            Debug.Log("Aucun policier assigné pour la résolution automatique.");
            return;
        }

        ResolveEvent(currentBattle);
        UpdateEventDisplay();
    }

    public void ResolveEvent(EventBattle eventBattle)
    {
        if (cardMiddle == null)
        {
            Debug.LogWarning("Aucun policier assigné à cet événement.");
            return;
        }

        // Calcul des chances de succès
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
            Debug.Log($"Succès : {eventBattle.eventName}");
            eventBattle.isResolved = true;
            
            EventManager.Instance.ResolveEvent(eventBattle);
            ClosePanel();
        }
        else
        {
            Debug.Log($"Échec : {eventBattle.eventName}");
            eventBattle.remainingAttempts--;

            if (eventBattle.remainingAttempts <= 0)
            {
                Debug.Log($"Événement échoué définitivement : {eventBattle.eventName}");
                Debug.Log($"{cardMiddle.cardName} a été blessé !");
            }
        }
        UpdateEventDisplay();
    }

    public void DebugTestMacro()
    {
        if (currentBattle == null || cardMiddle == null)
        {
            return; 
        }
        ResolveEvent(currentBattle);
    }
}
