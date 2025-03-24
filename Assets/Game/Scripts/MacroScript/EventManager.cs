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
        if (eventSlots.Count == 0)
        {
            Debug.LogWarning("eventSlots est vide ! Vérifie l'initialisation.");
        }

        GameManager.AssignStartingEvent(listEvent);
        AssignEvents();
    }


    public void AssignEvents()
    {
        List<EventBattle> eventList = GameManager.listEvent.eventBattles;

        foreach (EventBattle battle in eventList)
        {
            if (eventLocations.TryGetValue(battle.location, out EventDisplay display))
            {
                display.SetEvent(battle);
                Debug.Log(battle.location);

                //  Ajout dans eventSlots (pour ResolveEvent)
                EventSlot slot = eventSlots.Find(s => s.locationName == battle.location);
                if (slot == null)
                {
                    slot = new EventSlot
                    {
                        locationName = battle.location,
                        eventDisplay = display,
                        currentEvent = battle
                    };
                    eventSlots.Add(slot);
                }
                else
                {
                    slot.currentEvent = battle;
                }
            }
            else
            {
                Debug.LogWarning($"Lieu non trouvé : {battle.location}");
            }
        }
    }

    public void ResolveEvent(EventBattle eventBattle)
    {
        // Étape 1 : Vérifier si l'événement existe dans la liste et le retirer
        if (listEvent.eventBattles.Contains(eventBattle))
        {
            listEvent.eventBattles.Remove(eventBattle);
            Debug.Log($"Événement '{eventBattle.eventName}' supprimé de la liste.");
        }
        else
        {
            Debug.LogWarning($"Événement '{eventBattle.eventName}' non trouvé dans la liste.");
        }

        // Étape 2 : Ajouter le nextEvent s'il existe
        if (eventBattle.nextEvent != null)
        {
            EventBattle nextBattle = (EventBattle)eventBattle.nextEvent;
            listEvent.eventBattles.Add(nextBattle);
            Debug.Log($"Événement '{nextBattle.eventName}' ajouté à la liste.");
        }
        Debug.Log(listEvent.eventBattles);
        // Étape 3 : Réassigner les événements
        AssignEvents();
    }

    public void AutoResolveRemainingEvents()
    {

        foreach (var kvp in eventLocations)
        {
            EventDisplay display = kvp.Value;
            EventBattle battle = display.currentBattle;
            if (battle == null) continue;
            // Vérifie que l'événement existe, n'a pas été résolu et qu'il reste des tentatives
            if (!battle.isResolved)
            {
                if (battle.remainingAttempts <= 0)
                {
                    Debug.Log($"L'événement '{battle.eventName}' se déclenche automatiquement (échecs cumulés).");
                    battle.isResolved = false;  // Considéré comme échec automatique
                }
                else
                {
                    // Simule une résolution automatique (comme dans `ResolveEvent` d'EventDisplay)
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
                        Debug.Log($"Succès automatique : {battle.eventName}");
                        battle.isResolved = true;
                        ResolveEvent(battle);
                    }
                    else
                    {
                        Debug.Log($"Échec automatique : {battle.eventName}");
                        battle.remainingAttempts--;
                    }
                }
            }
            else
            {
                if (GameManager.WinBattle == true)
                {
                    ResolveEvent(battle);
                }
                else
                {
                    battle.remainingAttempts--;
                }
            }
        }
        Debug.Log("Fin de la résolution automatique des événements.");
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
