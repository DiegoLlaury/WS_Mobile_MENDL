using UnityEngine;
using System.Collections.Generic;
using WS_DiegoCo_Event;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    // Référence à tous les lieux et leurs événements en cours
    public Dictionary<EventBattle.EventPlace, EventBattle> activeEvents = new Dictionary<EventBattle.EventPlace, EventBattle>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AssignEventToPlace(EventBattle eventBattle)
    {
        if (!activeEvents.ContainsKey(eventBattle.eventPlace))
        {
            activeEvents[eventBattle.eventPlace] = eventBattle;
            Debug.Log($"Événement '{eventBattle.eventName}' assigné à {eventBattle.eventPlace}");
        }
        else
        {
            Debug.LogWarning($"Un événement est déjà en cours au {eventBattle.eventPlace}");
        }
    }

    public void ResolveEvent(EventBattle eventBattle)
    {
        if (activeEvents.ContainsKey(eventBattle.eventPlace))
        {
            if (eventBattle.isResolved)
            {
                Debug.Log($"Événement '{eventBattle.eventName}' terminé à {eventBattle.eventPlace}");

                // Remplacer l'événement par le suivant s'il existe
                if (eventBattle.nextEvent is EventBattle nextEvent)
                {
                    Debug.Log($"Nouveau événement déclenché : {nextEvent.eventName} au {nextEvent.eventPlace}");
                    activeEvents[eventBattle.eventPlace] = nextEvent;
                }
                else
                {
                    activeEvents.Remove(eventBattle.eventPlace);
                    Debug.Log($"Aucun nouvel événement pour {eventBattle.eventPlace}");
                }
            }
        }
        else
        {
            Debug.LogWarning($"Aucun événement trouvé au {eventBattle.eventPlace}");
        }
    }
}
