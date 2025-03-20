using UnityEngine;
using System.Collections.Generic;
using WS_DiegoCo_Event;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    // R�f�rence � tous les lieux et leurs �v�nements en cours
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
            Debug.Log($"�v�nement '{eventBattle.eventName}' assign� � {eventBattle.eventPlace}");
        }
        else
        {
            Debug.LogWarning($"Un �v�nement est d�j� en cours au {eventBattle.eventPlace}");
        }
    }

    public void ResolveEvent(EventBattle eventBattle)
    {
        if (activeEvents.ContainsKey(eventBattle.eventPlace))
        {
            if (eventBattle.isResolved)
            {
                Debug.Log($"�v�nement '{eventBattle.eventName}' termin� � {eventBattle.eventPlace}");

                // Remplacer l'�v�nement par le suivant s'il existe
                if (eventBattle.nextEvent is EventBattle nextEvent)
                {
                    Debug.Log($"Nouveau �v�nement d�clench� : {nextEvent.eventName} au {nextEvent.eventPlace}");
                    activeEvents[eventBattle.eventPlace] = nextEvent;
                }
                else
                {
                    activeEvents.Remove(eventBattle.eventPlace);
                    Debug.Log($"Aucun nouvel �v�nement pour {eventBattle.eventPlace}");
                }
            }
        }
        else
        {
            Debug.LogWarning($"Aucun �v�nement trouv� au {eventBattle.eventPlace}");
        }
    }
}
