using UnityEngine;
using System.Collections.Generic;
using WS_DiegoCo_Event;
using System.Diagnostics.Tracing;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    // R�f�rence � tous les lieux et leurs �v�nements en cours
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
                    Debug.Log($"�v�nement '{eventBattle.eventName}' termin� � {slot.locationName}");

                    // Passe � l��v�nement suivant s�il existe
                    if (eventBattle.nextEvent != null)
                    {
                        slot.currentEvent = (EventBattle)eventBattle.nextEvent;
                        slot.eventDisplay.SetEvent(slot.currentEvent);
                        Debug.Log($"Nouveau �v�nement : {slot.currentEvent.eventName} � {slot.locationName}");
                    }
                    else
                    {
                        slot.currentEvent = null;
                        Debug.Log($"Aucun nouvel �v�nement pour {slot.locationName}");
                    }
                }
                return;
            }
        }
        Debug.LogWarning("�v�nement non trouv� dans les slots !");
    }

    public void AutoResolveRemainingEvents()
    {
        Debug.Log("D�but de la r�solution automatique des �v�nements non r�alis�s...");

        foreach (var kvp in eventLocations)
        {
            EventDisplay display = kvp.Value;
            EventBattle battle = display.currentBattle;

            // V�rifie que l'�v�nement existe, n'a pas �t� r�solu et qu'il reste des tentatives
            if (battle != null && !battle.isResolved)
            {
                if (battle.remainingAttempts <= 0)
                {
                    Debug.Log($"L'�v�nement '{battle.eventName}' se d�clenche automatiquement (�checs cumul�s).");
                    battle.isResolved = false;  // Consid�r� comme �chec automatique
                }
                else
                {
                    // Simule une r�solution automatique (comme dans `ResolveEvent` d'EventDisplay)
                    int baseSuccessChance = battle.eventDifficulty switch
                    {
                        EventBattle.EventDifficulty.Facile => 70,
                        EventBattle.EventDifficulty.Moyen => 50,
                        EventBattle.EventDifficulty.Difficile => 30,
                        _ => 50
                    };

                    bool success = Random.Range(0, 100) < baseSuccessChance;

                    if (success)
                    {
                        Debug.Log($"Succ�s automatique : {battle.eventName}");
                        battle.isResolved = true;
                        ResolveEvent(battle);
                    }
                    else
                    {
                        Debug.Log($"�chec automatique : {battle.eventName}");
                        battle.remainingAttempts--;
                    }
                }
            }
        }
        Debug.Log("Fin de la r�solution automatique des �v�nements.");
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
                Debug.Log($"L'�v�nement '{display.currentBattle.eventName}' n'a pas encore de carte assign�e !");
                return false;
            }
        }
        Debug.Log("Tous les �v�nements sont pr�ts !");
        return true;
    }
}
