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
        public bool corruption;
        public int health;
        public int damageMin;
        public int damageMax;
        public List<DamageType> damageType;
        public Sprite cardImage;
        
        public enum CardType
        {
            Fire,
            
            Earth,
           
            Water,
            
            Air,

            Dark,

            Light
        }

        public enum DamageType
        {
            Fire,

            Earth,

            Water,

            Air,

            Dark,

            Light
        }
    }
}
