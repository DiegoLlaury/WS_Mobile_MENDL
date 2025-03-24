using System.Collections.Generic;
using UnityEngine;
using WS_DiegoCo_Enemy;
using WS_DiegoCo_Middle;


namespace WS_DiegoCo_Event
{
    [CreateAssetMenu(fileName = "New Event", menuName = "Event")]
    public class EventBattle : ScriptableObject
    {
        public EventType eventType;
        public EventDifficulty eventDifficulty;

        public int numberTurn;
        public int currentTurn;
        public int conditionNumber;
        
        public List<Enemy> enemies;
        public ScriptableObject nextEvent;

        public bool boss;
        
        public string description;
        public string eventName;
        public string location;
        
        public Sprite background;

        public int remainingAttempts = 2; // Pour gérer les échecs répétés
        public int MaxAttempts;
        public bool isResolved = true;

        public enum EventType
        {
            Combat,

            Infiltration,

            Enquete
        }

        public enum EventDifficulty
        {
            Facile,

            Moyen,

            Difficile
        }
    }
}

