using UnityEngine;
using WS_DiegoCo_Event;
using WS_DiegoCo_Middle;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
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
    public TMP_Text textError;
 
    public Image cardImage;
    public Image cardImageWorld;
    public Image cardBorder;
    public Image buildingImage;
    public Image backgroundImage;
    public Image errorPanel;
    private static int numberOfPlayer = 0;
    private float probVar;

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
            percentagePlayerText.text = $"{probVar.ToString()} %";
            percentagePlayerTextWorld.text = $"{probVar.ToString()} %";
        }  
        if (numberOfTurn != null) numberOfTurn.text = currentBattle.numberTurn.ToString();
        if (backgroundImage != null) backgroundImage.sprite = currentBattle.background;


        if (cardMiddle != null && cardImage != null)
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
            textError.text = new string("Il n'y a pas d'agent assigner.");
            StartCoroutine(ErrorPanel());
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
        cardBorder.gameObject.SetActive(false);
        percentagePlayerText.gameObject.SetActive(false);

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
                    probVar = GameManager.CalculatedWinChance(currentBattle);
                    cardImage.sprite = cardMiddle.cardImage;
                    percentagePlayerText.text = $"{probVar.ToString()} %";
                    percentagePlayerTextWorld.text = $"{probVar.ToString()} %";
                    cardImageWorld.gameObject.SetActive(true);
                    cardImageWorld.transform.Find("CardMiddleImage").GetComponent<Image>().sprite = cardPlayer.cardImage;
                    if (cardPlayer.symbolTypes == CardMiddle.SymbolTypes.Heart || cardPlayer.symbolTypes == CardMiddle.SymbolTypes.Square)
                    {
                        cardImage.color = new Color32(173, 3, 3, 255);
                        cardImageWorld.transform.Find("CardMiddleImage").GetComponent<Image>().color = new Color32(173, 3, 3, 255);
                    }
                    else
                    {
                        cardImage.color = Color.black;
                        cardImageWorld.transform.Find("CardMiddleImage").GetComponent<Image>().color = Color.black;
                    }
                    cardBorder.gameObject.SetActive(true);
                    percentagePlayerTextWorld.gameObject.SetActive(true);
                    percentagePlayerText.gameObject.SetActive(true);
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
            textError.text = new string("Un policier est déjà assigné à cet événement !");
            StartCoroutine(ErrorPanel());
        }
    }

    public void StartEvent()
    {
        if (!EventManager.Instance.AreAllEventsReady())
        {
            textError.text = new string("Tous les événements n'ont pas encore de carte assignée !");
            StartCoroutine(ErrorPanel());
            return;
        }

        if (cardMiddle != null)
        {
            currentBattle.isResolved = false;
            
            GameManager.StartEvent(cardMiddle, currentBattle);
        }

    }

    private IEnumerator ErrorPanel()
    {

        // Assure-toi que le panneau est visible au début
        errorPanel.gameObject.SetActive(true);

        // Rendre le panneau complètement opaque
        Color panelColor = errorPanel.color;
        panelColor.a = 1f;
        errorPanel.color = panelColor;

        Color textColor = textError.color;
        textColor.a = 1f;
        textError.color = textColor;

        // Attendre 1 seconde
        yield return new WaitForSeconds(1f);

        // Disparition progressive sur 0.8 secondes
        float duration = 0.8f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);

            panelColor.a = alpha;
            errorPanel.color = panelColor;

            textColor.a = alpha;
            textError.color = textColor;

            yield return null;
        }

        // Cache le panneau à la fin de l’animation
        errorPanel.gameObject.SetActive(false);
    }
}
