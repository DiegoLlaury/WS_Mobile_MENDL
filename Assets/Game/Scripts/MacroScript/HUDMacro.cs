using UnityEngine;
using TMPro;
public class HUDMacro : MonoBehaviour
{
    public static HUDMacro Instance;


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
        UpdateHUDMacro();
    }

    public void UpdateHUDMacro()
    {
        numberOfTurnText.text = GameManager.turnToKeep.ToString();
    }

    public void PassTurn()
    {
        GameManager.turnToKeep--;
        UpdateHUDMacro();
    }
}
