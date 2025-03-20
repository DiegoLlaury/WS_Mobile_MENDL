using UnityEngine;
using WS_DiegoCo_Middle;
using WS_DiegoCo_Event;

public static class GameManager 
{
    //public static GameManager Instance { get; private set; }
    public static CardMiddle selectedCard;
    public static EventBattle currentEvent;

    public static void AssignPoliceToEvent(EventBattle eventBattle, CardMiddle officer)
    {
        if (eventBattle.assignedOfficer == null)
        {
            eventBattle.assignedOfficer = officer;
            Debug.Log($"{officer.cardName} assigné à l’événement {eventBattle.eventName}");
        }
        else
        {
            Debug.Log("Un policier est déjà assigné à cet événement.");
        }
    }

    public static void ResolveEvent(EventBattle eventBattle)
    {
        if (eventBattle.assignedOfficer == null)
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

        int totalChance = baseSuccessChance + eventBattle.assignedOfficer.skillLevel;
        bool success = Random.Range(0, 100) < totalChance;

        if (success)
        {
            Debug.Log($"Succès : {eventBattle.eventName}");
            eventBattle.isResolved = true;

            // Déclenche l'événement suivant s'il existe
            if (eventBattle.nextEvent is EventBattle nextEvent)
            {
                Debug.Log($"Nouvel événement déclenché : {nextEvent.eventName}");
            }
        }
        else
        {
            Debug.Log($"Échec : {eventBattle.eventName}");
            eventBattle.remainingAttempts--;

            if (eventBattle.remainingAttempts <= 0)
            {
                Debug.Log($"Événement échoué définitivement : {eventBattle.eventName}");
                Debug.Log($"{eventBattle.assignedOfficer.cardName} a été blessé !");
            }
        }
    }

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
