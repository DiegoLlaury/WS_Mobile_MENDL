using UnityEngine;
using WS_DiegoCo_Middle;
using WS_DiegoCo_Event;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public CardMiddle selectedCard;
    public EventBattle currentEvent;
    

    private void Awake()
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
    }

    private void Start()
    {
        StartEvent(selectedCard, currentEvent);
    }

    public EventBattle GetEvent()
    {
        return currentEvent;
    }

    public void StartEvent(CardMiddle card, EventBattle eventData)
    {
        selectedCard = card;
        currentEvent = eventData;
        Debug.Log(currentEvent);
        //UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");

        switch (currentEvent.eventType)
        {
            case EventBattle.EventType.Combat:
                EnemyManager.Instance.StartCombat(eventData);
                break;
            case EventBattle.EventType.Infiltration:
                InfiltrationMode.Instance.StartInfiltration(eventData);
                break;
            case EventBattle.EventType.Enquete:
                InvestigationMode.Instance.StartInvestigation(eventData);
                break;
            default:
                Debug.LogError("Unknown event type");
                break;
        }
    }
}
