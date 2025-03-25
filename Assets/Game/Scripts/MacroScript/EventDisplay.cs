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
    public TMP_Text numberOfTurn;
    public TMP_Text percentagePlayerText;
    public TMP_Text percentagePlayerTextWorld;

    public Image cardImage;
    public Image cardImageWorld;
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
        //currentBattle.affectedCharacter = null;
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
        if (typeEventText != null)
        {
            typeEventText.text = $"{currentBattle.eventType.ToString()} / {currentBattle.eventDifficulty.ToString()}";
        }
        if (cardMiddle != null && percentagePlayerText != null)
        {
            percentagePlayerText.text = $"{cardMiddle.skillLevel.ToString()} %";
            percentagePlayerTextWorld.text = $"{cardMiddle.skillLevel.ToString()} %";
        }  
        if (numberOfTurn != null) numberOfTurn.text = currentBattle.numberTurn.ToString();
        if (backgroundImage != null) backgroundImage.sprite = currentBattle.background;


        if (cardMiddle != null && cardImage != null)
        {
            cardImage.sprite = cardMiddle != null ? cardMiddle.cardImage : null; 
            cardImageWorld.sprite = cardMiddle != null ? cardMiddle.cardImage : null;
        }
    }

    public void RefreshDisplay()
    {
        UpdateEventDisplay();
    }

    public void SetEvent(EventBattle eventBattle)
    {
        if (eventBattle == null)
        {
            Debug.LogError("EventBattle is null!");
            return;
        }

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
        percentagePlayerTextWorld.gameObject.SetActive(false);
        cardImageWorld.gameObject.SetActive(false);
        numberOfPlayer--;
    }

    public void SetPlayer(CardMiddle cardPlayer)
    {
        if (cardPlayer == null)
        {
            Debug.LogError("cardPlayer est null dans SetPlayer !");
            return;
        }

        if (cardMiddle == null)
        {
            cardMiddle = cardPlayer;

            if (cardImage != null)
            {
                if (cardPlayer.cardImage != null)
                {
                    cardImage.sprite = cardPlayer.cardImage;
                    cardImageWorld.sprite = cardPlayer.cardImage;
                    percentagePlayerText.text = $"{cardMiddle.skillLevel.ToString()} %";
                    percentagePlayerTextWorld.text = $"{cardMiddle.skillLevel.ToString()} %";
                    cardImageWorld.gameObject.SetActive(true);
                    percentagePlayerTextWorld.gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogWarning("La carte n’a pas d’image assignée !");
                }
            }
            else
            {
                Debug.LogError("cardImage n’est pas assigné dans l’inspecteur !");
            }

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
