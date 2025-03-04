using UnityEngine;
using System.Collections.Generic;

namespace WS_DiegoCo_Enemy
{
    [CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
    public class Enemy : ScriptableObject
    {
        public string enemyName;
        public int maxHealth;
        public int health;
        public int defense;
        public int damage;
        public int discretion;
        public int perception;
        public Sprite enemyImage;
    }
}


