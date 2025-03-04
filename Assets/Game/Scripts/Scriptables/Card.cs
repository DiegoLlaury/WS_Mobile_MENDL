using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WS_DiegoCo
{
    [CreateAssetMenu(fileName = "New Card", menuName = "Card")]
    public class Card : ScriptableObject
    {
        public string cardName;
        public List<CardType> cardType;
        public List<StatType> statType;
        public int health;
        public int defense;
        public int damage;
        public int discretion;
        public int perception;
        public int energy;
        public string effect;
        public Sprite cardImage;

        public List<CardEffect> effects;
        
        public enum CardType
        {
            Coeur,
            
            Pique,
           
            Carreau,
            
            Trefle
        }

        public enum StatType
        {
            health,

            defense,

            damage,

            discretion,

            perception

        }
    }
}
