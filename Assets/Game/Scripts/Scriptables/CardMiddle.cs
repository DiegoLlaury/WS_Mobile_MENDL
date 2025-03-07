using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static WS_DiegoCo.Card;

namespace WS_DiegoCo_Middle
{
    [CreateAssetMenu(fileName = "New CardMiddle", menuName = "CardMiddle")]
    public class CardMiddle : ScriptableObject
    {
        public string cardName;
        public string description;
        public List<StatType> statType;
        public int health;
        public int maxHealth;
        public int maxShield;
        public int shield;
        public int maxDamage;
        public int damage;
        public int maxDiscretion;
        public int discretion;
        public int maxPerception;
        public int perception;
        public Sprite cardImage;

       
    }
}