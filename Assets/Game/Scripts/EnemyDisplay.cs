using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WS_DiegoCo_Enemy;
using System;
using WS_DiegoCo;

public class EnemyDisplay : MonoBehaviour
{

    public Enemy enemyData;

    public Sprite enemyImageDisplay;
    public TMP_Text nameText;
    public TMP_Text healthText;
    public TMP_Text damageText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyData.health = enemyData.maxHealth;
        UpdateEnemyDisplay();
    }

    private void UpdateEnemyDisplay()
    {
        nameText.text = enemyData.enemyName;
        healthText.text = enemyData.health.ToString();
        damageText.text = enemyData.damage.ToString();
        enemyImageDisplay = enemyData.enemyImage;
    }

    public void TakeDamage(int damage)
    {
        enemyData.health -= damage; // Modify the ScriptableObject's health
        UpdateEnemyDisplay(); // Refresh UI
        Debug.Log($"Enemy {enemyData.enemyName} took {damage} damage. Remaining health: {enemyData.health}");
    }

    public void ModifyStat(Card.StatType stat, int amount)
    {
        switch (stat)
        {
            case Card.StatType.health:
                enemyData.health += amount;
                break;
            case Card.StatType.defense:
                enemyData.defense += amount;
                break;
            case Card.StatType.damage:
                enemyData.damage += amount;
                break;
            case Card.StatType.discretion:
                enemyData.discretion += amount;
                break;
            case Card.StatType.perception:
                enemyData.perception += amount;
                break;
        }
        UpdateEnemyDisplay();
        Debug.Log($"Enemy {stat} changed by {amount}");
    }
}
