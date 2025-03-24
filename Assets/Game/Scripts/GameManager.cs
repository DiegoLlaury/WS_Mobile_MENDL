using UnityEngine;
using WS_DiegoCo_Middle;
using WS_DiegoCo_Event;
using NUnit.Framework;
using System.Collections.Generic;

public static class GameManager
{
    public static CardMiddle selectedCard;
    public static EventBattle currentEvent;
    public static ListEvent listEvent;

    public static bool firstTime = true;
    public static bool WinBattle;

    private static List<EventDisplay> locationDisplay = new List<EventDisplay>();

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
        
        UnityEngine.SceneManagement.SceneManager.LoadScene("MacroScene");
        EventManager.Instance.AutoResolveRemainingEvents();
    }

    public static void AssignStartingEvent(ListEvent eventList)
    {
        if (firstTime)
        {
            listEvent = eventList;
            firstTime = false;
        }   
    }
}
