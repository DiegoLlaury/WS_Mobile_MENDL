using UnityEngine;
using System.Collections.Generic;
using WS_DiegoCo_Event;
using System.Diagnostics.Tracing;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    [SerializeField] private ListEvent listEvent;
    [SerializeField] private List<string> locationNames = new List<string>();
    [SerializeField] private List<EventDisplay> locationDisplay = new List<EventDisplay>();

    [SerializeField] public Dictionary<string, EventDisplay> eventLocations = new Dictionary<string, EventDisplay>();
    private List<EventBattle> currentEventBattles;



    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        RefreshEventLocations();
    }

    private void Start()
    {
        if (GameManager.firstTime == true)
        {
            GameManager.AssignStartingEvent(listEvent);
            currentEventBattles = new List<EventBattle>(listEvent.eventBattles);
        }
        else
        {
            currentEventBattles = new List<EventBattle>(listEvent.eventBattles);
        }
        AssignEvents();


        //currentsEvents = GameManager.listEvent.eventBattles;
    }

    public void RefreshEventLocations()
    {
        eventLocations.Clear();
        for (int i = 0; i < locationNames.Count; i++)
        {
            if (!eventLocations.ContainsKey(locationNames[i]))
            {
                eventLocations.Add(locationNames[i], locationDisplay[i]);
            }
        }
    }
    public void SetEvents(string location, EventDisplay newEvent)
    {
        if (eventLocations.ContainsKey(location))
        {
            eventLocations[location] = newEvent;  // Met � jour si l'�v�nement existe d�j�
        }
        else
        {
            eventLocations.Add(location, newEvent);  // Ajoute si le lieu n'existe pas
        }
    }

    public void ClearEvents()
    {
        eventLocations.Clear();
    }

    public void AssignEvents()
    {
        RefreshEventLocations();

        if (currentEventBattles == null || currentEventBattles.Count == 0)
        {
            Debug.LogWarning("Aucun �v�nement � assigner.");
            return;
        }

        // Assigner les �v�nements aux lieux
        foreach (EventBattle battle in currentEventBattles)
        {
            Debug.Log($"Tentative d'assigner l'�v�nement '{battle.eventName}' au lieu : {battle.location}");
            EventDisplay display = GetEventDisplayByLocation(battle.location);
            if (display != null)
            {
                display.SetEvent(battle);
                Debug.Log($"�v�nement '{battle.eventName}' assign� � {battle.location}.");
            }
            else
            {
                Debug.LogWarning($"Aucun display trouv� pour le lieu : {battle.location}");
            }
        }
        foreach (var display in locationDisplay)
        {
            display.RefreshDisplay();
        }
    }

    public EventBattle GetEventByLocation(string location)
    {
        EventDisplay display = GetEventDisplayByLocation(location);
        if (display != null)
        {
            return display.currentBattle;
        }

        Debug.LogWarning($"Aucun �v�nement trouv� pour le lieu : {location}");
        return null;
    }

    private EventDisplay GetEventDisplayByLocation(string location)
    {
        if (eventLocations.TryGetValue(location, out EventDisplay display))
        {
            return display;
        }
        Debug.LogWarning($"Aucun EventDisplay trouv� pour le lieu : {location}");
        return null;
    }

    public void ResolveEvent(EventBattle eventBattle)
    {
        if (currentEventBattles.Contains(eventBattle))
        {
            currentEventBattles.Remove(eventBattle);
            Debug.Log($"�v�nement '{eventBattle.eventName}' r�solu et retir� des �v�nements actuels.");
        }
        else
        {
            Debug.LogWarning($"�v�nement '{eventBattle.eventName}' non trouv� dans les �v�nements actuels.");
        }

        if (eventBattle.nextEvent != null)
        {
            EventBattle nextBattle = (EventBattle)eventBattle.nextEvent;
            currentEventBattles.Add(nextBattle);
            Debug.Log($"Nouveau �v�nement '{nextBattle.eventName}' ajout�.");
        }

        // R�assigner les �v�nements pour actualiser l'affichage
        AssignEvents();
    }

    public void AutoResolveRemainingEvents()
    {
        foreach (var kvp in new Dictionary<string, EventDisplay>(eventLocations))
        {
            EventDisplay display = kvp.Value;
            EventBattle battle = display.currentBattle;

            if (battle == null) continue;

            if (!battle.isResolved)
            {
                if (battle.remainingAttempts <= 0)
                {
                    Debug.Log($"L'�v�nement '{battle.eventName}' se d�clenche automatiquement (�checs cumul�s).");
                    battle.isResolved = false;
                }
                else
                {
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
    }


    public bool AreAllEventsReady()
    {
        foreach (var kvp in eventLocations)
        {
            EventDisplay display = kvp.Value;
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