using UnityEngine;
using WS_DiegoCo_Middle;
using WS_DiegoCo_Event;
using NUnit.Framework;
using System.Collections.Generic;

public static class GameManager
{
    public static CardMiddle selectedCard;
    public static EventBattle currentEvent;
    public static List<EventBattle> currentEventBattles;
    public static List<EventBattle> startingEventBattles;
    public static List<(string message, bool isVictory)> eventResults = new List<(string, bool)>();
    public static List<CardMiddle> generatedCharacter = new List<CardMiddle>();

    public static bool firstTime = true;
    public static bool WinBattle;
    public static int turnToKeep = 15;
    public static bool isGameStarted = false;

    private static List<EventDisplay> locationDisplay = new List<EventDisplay>();
    private static List<EventBattle> tempEvents = new List<EventBattle>();
    

    public static void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        if (scene.name == "SampleScene")
        {
            switch (currentEvent.eventType)
            {
                case EventBattle.EventType.Combat:
                    EnemyManager.Instance?.StartCombat(currentEvent);
                    break;
                case EventBattle.EventType.Infiltration:
                    InfiltrationMode.Instance?.StartInfiltration(currentEvent);
                    break;
                case EventBattle.EventType.Enquete:
                    InvestigationMode.Instance?.StartInvestigation(currentEvent);
                    break;
                default:
                    Debug.LogError("Unknown event type");
                    break;
            }
            // Se désinscrire après le lancement pour éviter des appels multiples
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        else if (scene.name == "MacroScene")
        {
            EventManager.Instance.AssignEvents();  // Recharger les événements
        }
    }

    public static void StartEvent(CardMiddle card, EventBattle eventData)
    {
        selectedCard = card;
        currentEvent = eventData;
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }

    public static void EndEvent()
    {
        tempEvents.Clear();

        string currentEventResult = WinBattle
        ? $"Victoire dans l'événement {currentEvent.eventType} : {currentEvent.eventName} !"
        : $"Défaite dans l'événement {currentEvent.eventType} : {currentEvent.eventName}...";

        eventResults.Add((currentEventResult, WinBattle));

        if (WinBattle)
        {
            currentEventBattles.Remove(currentEvent);
            tempEvents.Add(currentEvent.nextEvent);
        }
        else
        {
            if (currentEvent.remainingAttempts == 0)
            {
                currentEventBattles.Remove(currentEvent);
                tempEvents.Add(currentEvent.nextEvent);
            }
            else
            {
                currentEvent.remainingAttempts--;
                currentEventBattles.Remove(currentEvent);
                tempEvents.Add(currentEvent);
            }
        }

        foreach (EventBattle eventBattle in currentEventBattles)
        {
            if (eventBattle.affectedCharacter == null)
            {
                Debug.LogError($"EventBattle {eventBattle.eventName} n'a pas de personnage assigné !");
            }
            else
            {
                Debug.Log($"EventBattle {eventBattle.eventName} est bien assigné à {eventBattle.affectedCharacter.cardName}");
            }

            if (Random.Range(0, 100) > CalculatedWinChance(eventBattle))
            {
                eventResults.Add(($"Succès automatique de {eventBattle.eventType} : {eventBattle.eventName} !", true));
                tempEvents.Add(eventBattle.nextEvent);
            }
            else
            {
                if (eventBattle.remainingAttempts == 0)
                {
                    eventResults.Add(($"Échec définitif de {eventBattle.eventType} : {eventBattle.eventName}...", false));
                    tempEvents.Add(eventBattle.nextEventFail);
                }
                else
                {
                    eventResults.Add(($"Échec de {eventBattle.eventType} : {eventBattle.eventName}, essais restants : {eventBattle.remainingAttempts}", false));
                    eventBattle.remainingAttempts--;
                    tempEvents.Add(eventBattle);
                }
            }
        }

        currentEventBattles.Clear();

        foreach (EventBattle eventBattle1 in tempEvents)
        {
            currentEventBattles.Add(eventBattle1);
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("MacroScene");
        HUDMacro.Instance.PassTurn();
    }

    public static float CalculatedWinChance(EventBattle battle)
    {
        if (battle.affectedCharacter == null)
        {
            Debug.LogError("La carte affectée est nulle.");
            return 0f;
        }

        int cardValue = 0;

        switch (battle.eventType)
        {
            case EventBattle.EventType.Infiltration:
                if(battle.affectedCharacter.symbolTypes == CardMiddle.SymbolTypes.Square)
                {
                    cardValue = battle.affectedCharacter.square;
                }
                break;
            case EventBattle.EventType.Enquete:
                if (battle.affectedCharacter.symbolTypes == CardMiddle.SymbolTypes.Clover)
                {
                    cardValue = battle.affectedCharacter.clover;
                }
                break;
            case EventBattle.EventType.Combat:
                if (battle.affectedCharacter.symbolTypes == CardMiddle.SymbolTypes.Spade)
                {
                    cardValue = battle.affectedCharacter.spade;
                }
                break;
        }
         // Utilisez la valeur appropriée selon votre logique

        // Assurez-vous que cardValue est supérieur ou égal à 10
        cardValue = Mathf.Max(cardValue, 10);

        float baseChance = 0f;

        switch (battle.eventDifficulty)
        {
            case EventBattle.EventDifficulty.Facile:
                baseChance = 70f;
                break;
            case EventBattle.EventDifficulty.Moyen:
                baseChance = 50f;
                break;
            case EventBattle.EventDifficulty.Difficile:
                baseChance = 30f; // Base de 50%
                break;
            default:
                Debug.LogError("Type d'événement non pris en charge.");
                return 0f;
        }

        // Augmentation de la chance par point au-dessus de 10
        float additionalChance = (cardValue - 10) * 10f;
        float totalChance = baseChance + additionalChance;

        // Limitez la probabilité entre 0% et 100%
        totalChance = Mathf.Clamp(totalChance, 0f, 100f);

        return totalChance;
    }

    public static void AssignStartingEvent(ListEvent eventList)
    {
        if (firstTime)
        {

            startingEventBattles = new List<EventBattle>(eventList.eventBattles);
            currentEventBattles = new List<EventBattle>(startingEventBattles);
            firstTime = false;
        }   
    }
}
