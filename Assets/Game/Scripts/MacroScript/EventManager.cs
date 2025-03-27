using UnityEngine;
using System.Collections.Generic;
using WS_DiegoCo_Event;
using System.Diagnostics.Tracing;
using TMPro;
using UnityEngine.SceneManagement;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    [SerializeField] private ListEvent listEvent;
    [SerializeField] private List<string> locationNames = new List<string>();
    [SerializeField] private List<EventDisplay> locationDisplay = new List<EventDisplay>();

    [SerializeField] public Dictionary<string, EventDisplay> eventLocations = new Dictionary<string, EventDisplay>();

    private EventBattle endEvent;

    public GameObject endPanel;
    public TMP_Text endTitle;
    public TMP_Text endDescription;
    public bool bossInEvent;

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
        if (GameManager.firstTime)
        {
            GameManager.AssignStartingEvent(listEvent);
        }
        foreach (EventBattle battle in GameManager.currentEventBattles)
        {
            if (battle.end)
            {
                endEvent = battle;
                TriggerEnd();
                return;
            }
        }
        foreach (EventBattle battle in GameManager.currentEventBattles)
        {
            if (battle.boss == true)
            {
                bossInEvent = true;
            }
        }
            AssignEvents();


        //currentsEvents = GameManager.listEvent.eventBattles;
    }

    private void TriggerEnd()
    {
        Debug.Log("YUVIBN?");
        endPanel.SetActive(true);
        GameManager.firstTime = true;
        endTitle.text = endEvent.eventName;
        endDescription.text = endEvent.description;
    }

    public void Replay()
    {
        SceneManager.LoadScene("MacroScene");
    }

    public void Menu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
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
            eventLocations[location] = newEvent;  // Met à jour si l'événement existe déjà
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

        if (GameManager.currentEventBattles == null || GameManager.currentEventBattles.Count == 0)
        {
            Debug.LogWarning("Aucun événement à assigner.");
            return;
        }

        foreach (EventBattle battle in GameManager.currentEventBattles)
        {
            Debug.Log($"Tentative d'assigner l'événement '{battle.eventName}' au lieu : {battle.location}");
            EventDisplay display = GetEventDisplayByLocation(battle.location);
            if (display != null)
            {
                display.SetEvent(battle);
                Debug.Log($"Événement '{battle.eventName}' assigné à {battle.location}.");
            }
            else
            {
                Debug.LogWarning($"Aucun display trouvé pour le lieu : {battle.location}");
            }
        }

        // Actualise l'affichage
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

        Debug.LogWarning($"Aucun événement trouvé pour le lieu : {location}");
        return null;
    }

    private EventDisplay GetEventDisplayByLocation(string location)
    {
        if (eventLocations.TryGetValue(location, out EventDisplay display))
        {
            return display;
        }
        Debug.LogWarning($"Aucun EventDisplay trouvé pour le lieu : {location}");
        return null;
    }

    public void ResolveEvent(EventBattle eventBattle)
    {
        if (GameManager.currentEventBattles.Contains(eventBattle))
        {
            GameManager.currentEventBattles.Remove(eventBattle);
            Debug.Log($"Événement '{eventBattle.eventName}' résolu et retiré des événements actuels.");
        }
        else
        {
            Debug.LogWarning($"Événement '{eventBattle.eventName}' non trouvé dans les événements actuels.");
        }

        if (eventBattle.nextEvent != null)
        {
            GameManager.currentEventBattles.Add(eventBattle.nextEvent);
            Debug.Log($"Nouveau événement '{eventBattle.nextEvent.eventName}' ajouté.");
        }

        // Réassigner les événements pour actualiser l'affichage
        AssignEvents();
    }

    //public void AutoResolveRemainingEvents()
    //{
    //    foreach (var kvp in new Dictionary<string, EventDisplay>(eventLocations))
    //    {
    //        EventDisplay display = kvp.Value;
    //        EventBattle battle = display.currentBattle;

    //        if (battle == null) continue;

    //        if (!battle.isResolved)
    //        {
    //            if (battle.remainingAttempts <= 0)
    //            {
    //                Debug.Log($"L'événement '{battle.eventName}' se déclenche automatiquement (échecs cumulés).");
    //                battle.isResolved = false;
    //            }
    //            else
    //            {
    //                int baseSuccessChance = battle.eventDifficulty switch
    //                {
    //                    EventBattle.EventDifficulty.Facile => 70,
    //                    EventBattle.EventDifficulty.Moyen => 50,
    //                    EventBattle.EventDifficulty.Difficile => 30,
    //                    _ => 50
    //                };

    //                bool success = Random.Range(0, 100) < baseSuccessChance;

    //                if (success)
    //                {
    //                    Debug.Log($"Succès automatique : {battle.eventName}");
    //                    battle.isResolved = true;
    //                    ResolveEvent(battle);
    //                }
    //                else
    //                {
    //                    Debug.Log($"Échec automatique : {battle.eventName}");
    //                    battle.remainingAttempts--;
    //                }
    //            }
    //        }
    //        else
    //        {
    //            if (GameManager.WinBattle == true)
    //            {
    //                ResolveEvent(battle);
    //            }
    //            else
    //            {
    //                battle.remainingAttempts--;
    //            }
    //        }
    //    }
    //}


    public bool AreAllEventsReady()
    {
        foreach (var kvp in eventLocations)
        {
            EventDisplay display = kvp.Value;
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