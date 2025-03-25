using UnityEngine;
using TMPro;
public class HUDMacro : MonoBehaviour
{
    public static HUDMacro Instance;

    public int turnMacro;
    public TMP_Text numberOfTurnText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    void Start()
    {
        turnMacro = 15;
        UpdateHUDMacro();
    }

    public void UpdateHUDMacro()
    {
        numberOfTurnText.text = turnMacro.ToString();
        GameManager.turnToKeep = turnMacro;
    }

    public void PassTurn()
    {
        turnMacro--;
        UpdateHUDMacro();
    }
}
