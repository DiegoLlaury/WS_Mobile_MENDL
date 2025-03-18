using UnityEngine;
using WS_DiegoCo_Middle;
using WS_DiegoCo_Event;

public static class GameManager 
{
    //public static GameManager Instance { get; private set; }
    public static CardMiddle selectedCard;
    public static EventBattle currentEvent;

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
    }

    public static void StartEvent(CardMiddle card, EventBattle eventData)
    {
        selectedCard = card;
        currentEvent = eventData;

        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }
}
