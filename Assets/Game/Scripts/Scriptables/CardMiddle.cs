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

        public int health;
        public int maxHealth;
        public int maxDefense;
        public int defense;
        public int maxStrenght;
        public int strenght;
        public int maxDiscretion;
        public int discretion;
        public int maxPerception;
        public int perception;

        public int heart;
        public int square;
        public int spade;
        public int clover;

        public int skillLevel;

        public Sprite cardImage;

        public enum SymbolTypes
        {
            Heart,

            Spade,

            Square,

            Clover
        }
       
    }
}