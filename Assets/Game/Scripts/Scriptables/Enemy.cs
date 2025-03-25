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
        public int maxDefense;
        public int defense;
        public int maxDamage;
        public int damage;
        public int maxDiscretion;
        public int discretion;
        public int maxPerception;
        public int perception;

        public Sprite enemyIdleImage;
        public Sprite enemyAttackImage;
        public Sprite enemyDamagedImage;

        public Sprite attackImage;
        public Sprite defenseImage;
        public Sprite buffImage;
        public Sprite debuffImage;
        public Sprite rondeImage;
        public Sprite questionImage;
    }
}


