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

    public static bool firstTime = true;
    public static bool WinBattle;
    public static int turnMacro = 15;

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
        turnMacro--;
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }

    public static void EndEvent()
    {
        tempEvents.Clear();
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
                tempEvents.Add(currentEvent.nextEvent);
            }
        }

        foreach (EventBattle eventBattle in currentEventBattles)
        {
            if (Random.Range(0, 100) > CalculatedWinChance(eventBattle))
            {
                tempEvents.Add(eventBattle.nextEvent);
            }
            else
            {
                if (eventBattle.remainingAttempts == 0)
                {
                    tempEvents.Add(eventBattle.nextEvent);
                }
                else
                {
                    eventBattle.remainingAttempts--;
                    tempEvents.Add(eventBattle.nextEvent);
                }
            }
        }

        currentEventBattles.Clear();

        foreach (EventBattle eventBattle1 in tempEvents)
        {
            currentEventBattles.Add(eventBattle1);
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("MacroScene");
    }

    private static float CalculatedWinChance(EventBattle battle)
    {
        return 50f;
    }

    public static void AssignStartingEvent(ListEvent eventList)
    {
        if (firstTime)
        {
            turnMacro = 15;
            startingEventBattles = new List<EventBattle>(eventList.eventBattles);
            currentEventBattles = new List<EventBattle>(startingEventBattles);
            firstTime = false;
        }   
    }
}
