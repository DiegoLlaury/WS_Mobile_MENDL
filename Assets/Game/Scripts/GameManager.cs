using UnityEngine;
using WS_DiegoCo_Middle;
using WS_DiegoCo_Event;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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
        StartBattle(selectedCard, currentEvent);
    }

    public void StartBattle(CardMiddle card, EventBattle eventData)
    {
        selectedCard = card;
        currentEvent = eventData;
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }
}
