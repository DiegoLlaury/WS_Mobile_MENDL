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
        public SymbolTypes symbolTypes;

        
        public int maxHealth;
        public int maxDefense;      
        public int maxStrenght;      
        public int maxDiscretion;  
        public int maxPerception;

        public int health;
        public int defense;
        public int strenght;
        public int perception;
        public int discretion;

        public int heart;
        public int square;
        public int spade;
        public int clover;

        public int skillLevel;
        public bool corruption;

        public Sprite cardImage;

        public enum SymbolTypes
        {
            Heart,

            Spade,

            Square,

            Clover
        }
        public bool statsGenerated = false;
    }
}