using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WS_DiegoCo_Enemy;

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
        UpdateEnemyDisplay();
    }

    private void UpdateEnemyDisplay()
    {
        nameText.text = enemyData.enemyName;
        healthText.text = enemyData.health.ToString();
        damageText.text = enemyData.damage.ToString();
        enemyImageDisplay = enemyData.enemyImage;
    }
}
