using UnityEngine;
using System.Collections.Generic;
using WS_DiegoCo_Event;


[CreateAssetMenu(fileName = "New ListEvent", menuName = "ListEvent")]
public class ListEvent : ScriptableObject
{
    public List<EventBattle> eventBattles;
}
