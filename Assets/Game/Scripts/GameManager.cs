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
            // Se d�sinscrire apr�s le lancement pour �viter des appels multiples
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        else if (scene.name == "MacroScene")
        {
            EventManager.Instance.AssignEvents();  // Recharger les �v�nements
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
        ? $"Victoire dans l'�v�nement {currentEvent.eventType} : {currentEvent.eventName} !"
        : $"D�faite dans l'�v�nement {currentEvent.eventType} : {currentEvent.eventName}...";

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
            if (Random.Range(0, 100) > CalculatedWinChance(eventBattle))
            {
                eventResults.Add(($"Succ�s automatique de {eventBattle.eventType} : {eventBattle.eventName} !", true));
                tempEvents.Add(eventBattle.nextEvent);
            }
            else
            {
                if (eventBattle.remainingAttempts == 0)
                {
                    eventResults.Add(($"�chec d�finitif de {eventBattle.eventType} : {eventBattle.eventName}...", false));
                    tempEvents.Add(eventBattle.nextEvent);
                }
                else
                {
                    eventResults.Add(($"�chec temporaire de {eventBattle.eventType} : {eventBattle.eventName}, tentatives restantes : {eventBattle.remainingAttempts}", false));
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

    private static float CalculatedWinChance(EventBattle battle)
    {
        return 50f;
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
