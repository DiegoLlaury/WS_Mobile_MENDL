using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class HUDMacro : MonoBehaviour
{
    public static HUDMacro Instance;


    public TMP_Text numberOfTurnText;
    public Transform gridHud;
    public GameObject popupPrefab;

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
        DisplayEventResults();
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

    private void DisplayEventResults()
    {
        foreach (var result in GameManager.eventResults)
        {
            GameObject popup = Instantiate(popupPrefab, gridHud);
            popup.GetComponent<PopupEventResult>().SetupPopup(result.message, result.isVictory);
        }

        GameManager.eventResults.Clear();
    }
    public void RefreshGrid()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(gridHud.GetComponent<RectTransform>());
    }
}
