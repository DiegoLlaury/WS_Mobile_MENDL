using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WS_DiegoCo
{
    [CreateAssetMenu(fileName = "New Card", menuName = "Card")]
    public class Card : ScriptableObject
    {
        public string cardName;
        public CardType cardType;
        public List<StatType> statType;
        public List<DropType> dropType;
        public int health;
        public int defense;
        public int damage;
        public int startingDamage;
        public int discretion;
        public int perception;
        public int energy;
        public string effect;
        public Sprite cardImage;

        public List<CardEffect> effects;
        
        public enum CardType
        {
            Heart,
            
            Spade,
           
            Square,
            
            Clover
        }

        public enum DropType
        {
            Attack,

            Competence
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
