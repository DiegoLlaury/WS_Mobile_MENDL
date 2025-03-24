using UnityEngine;
using System.Collections.Generic;
using WS_DiegoCo_Event;
using System.Diagnostics.Tracing;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    // Référence à tous les lieux et leurs événements en cours
    [System.Serializable]
    public class EventSlot
    {
        public string locationName;
        public EventDisplay eventDisplay;
        public EventBattle currentEvent;
    }

    [SerializeField] private List<EventSlot> eventSlots = new List<EventSlot>();
    [SerializeField] private ListEvent listEvent;
    [SerializeField] private List<string> locationNames = new List<string>();
    [SerializeField] private List<EventDisplay> locationDisplay = new List<EventDisplay>();
    [SerializeField] public Dictionary<string, EventDisplay> eventLocations = new Dictionary<string, EventDisplay>();

    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        for(int i =0; i < locationNames.Count; i++)
        {
            eventLocations.Add(locationNames[i], locationDisplay[i]);
        }
    }

    private void Start()
    {
        GameManager.AssignStartingEvent(listEvent);
        AssignEvents();
    }


    public void AssignEvents()
    {
        List<EventBattle> eventList = GameManager.listEvent.eventBattles;
        foreach (EventBattle battle in eventList)
        {
            eventLocations[battle.location].SetEvent(battle);
            Debug.Log(battle.location);
        }
    }

    public void ResolveEvent(EventBattle eventBattle)
    {
        foreach (EventSlot slot in eventSlots)
        {
            if (slot.currentEvent == eventBattle)
            {
                if (eventBattle.isResolved)
                {
                    Debug.Log($"Événement '{eventBattle.eventName}' terminé à {slot.locationName}");

                    // Passe à l’événement suivant s’il existe
                    if (eventBattle.nextEvent != null)
                    {
                        slot.currentEvent = (EventBattle)eventBattle.nextEvent;
                        slot.eventDisplay.SetEvent(slot.currentEvent);
                        Debug.Log($"Nouveau événement : {slot.currentEvent.eventName} à {slot.locationName}");
                    }
                    else
                    {
                        slot.currentEvent = null;
                        Debug.Log($"Aucun nouvel événement pour {slot.locationName}");
                    }
                }
                return;
            }
        }
        Debug.LogWarning("Événement non trouvé dans les slots !");
    }

    public EventBattle GetEventByLocation(string location)
    {
        foreach (EventSlot slot in eventSlots)
        {
            if (slot.locationName == location)
            {
                return slot.currentEvent;
            }
        }
        return null;
    }

    public bool AreAllEventsReady()
    {
        EventDisplay[] displays = Object.FindObjectsByType<EventDisplay>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (EventDisplay display in displays)
        {
            if (display.currentBattle != null && display.cardMiddle == null)
            {
                Debug.Log($"L'événement '{display.currentBattle.eventName}' n'a pas encore de carte assignée !");
                return false;
            }
        }
        Debug.Log("Tous les événements sont prêts !");
        return true;
    }
}
