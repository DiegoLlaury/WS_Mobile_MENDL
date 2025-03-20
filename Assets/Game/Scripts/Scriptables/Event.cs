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
        public EventPlace eventPlace;
        public EventDifficulty eventDifficulty;

        public int numberTurn;
        public int currentTurn;
        public int conditionNumber;
        
        public List<Enemy> enemies;
        public ScriptableObject nextEvent;

        public bool boss;
        public bool winFight = false;
        
        public string description;
        public string eventName;  
        
        public Sprite background;

        public int remainingAttempts = 2; // Pour gérer les échecs répétés
        public bool isResolved = false;

        public enum EventType
        {
            Combat,

            Infiltration,

            Enquete
        }

        public enum EventPlace
        {
            Comissariat,
            Prison,
            Gare,
            Hopital,
            Villa,
            Casino,
            Banque,
            Bar,
            Entrepots,
            Diner
        }

        public enum EventDifficulty
        {
            Facile,

            Moyen,

            Difficile
        }
    }
}

