using System.Collections.Generic;
using UnityEngine;
using WS_DiegoCo_Enemy;


namespace WS_DiegoCo_Event
{
    [CreateAssetMenu(fileName = "New Event", menuName = "Event")]
    public class EventBattle : ScriptableObject
    {
        public EventType eventType;
        public EventPlace eventPlace;
        public int numberTurn;
        public int conditionNumber;
        public List<Enemy> enemies;
        public bool boss;
        public string description;
        public string eventName;
        public Sprite background;

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
  
    }
}

