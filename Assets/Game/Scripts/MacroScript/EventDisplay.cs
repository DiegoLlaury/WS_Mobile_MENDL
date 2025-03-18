using UnityEngine;
using WS_DiegoCo_Event;
using WS_DiegoCo_Middle;
using UnityEngine.UI;
using TMPro;

public class EventDisplay : MonoBehaviour
{
    public EventBattle eventBattle;
    private CardMiddle cardMiddle;
    public TMP_Text description;
    public TMP_Text nameEvent;
    public TMP_Text typeEvent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void SetPlayer(CardMiddle cardPlayer)
    {
        Debug.Log(cardPlayer.name);
        cardMiddle = cardPlayer;
    }

    public void StartBattle()
    {
        GameManager.StartEvent(cardMiddle, eventBattle);
    }
}
