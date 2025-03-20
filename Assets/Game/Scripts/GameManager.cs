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
            Debug.Log($"{officer.cardName} assign� � l��v�nement {eventBattle.eventName}");
        }
        else
        {
            Debug.Log("Un policier est d�j� assign� � cet �v�nement.");
        }
    }

    public static void ResolveEvent(EventBattle eventBattle)
    {
        if (eventBattle.assignedOfficer == null)
        {
            Debug.LogWarning("Aucun policier assign� � cet �v�nement.");
            return;
        }

        // Calcul des chances de succ�s
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
            Debug.Log($"Succ�s : {eventBattle.eventName}");
            eventBattle.isResolved = true;

            // D�clenche l'�v�nement suivant s'il existe
            if (eventBattle.nextEvent is EventBattle nextEvent)
            {
                Debug.Log($"Nouvel �v�nement d�clench� : {nextEvent.eventName}");
            }
        }
        else
        {
            Debug.Log($"�chec : {eventBattle.eventName}");
            eventBattle.remainingAttempts--;

            if (eventBattle.remainingAttempts <= 0)
            {
                Debug.Log($"�v�nement �chou� d�finitivement : {eventBattle.eventName}");
                Debug.Log($"{eventBattle.assignedOfficer.cardName} a �t� bless� !");
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
            // Se d�sinscrire apr�s le lancement pour �viter des appels multiples
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
