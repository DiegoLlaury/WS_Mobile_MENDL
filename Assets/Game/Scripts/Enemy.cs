using UnityEngine;
using System.Collections.Generic;

namespace WS_DiegoCo_Enemy
{
    [CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
    public class Enemy : ScriptableObject
    {
        public string enemyName;
        public int health;
        public int damage;
        public Sprite enemyImage;
    }
}


