using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutoHUD : MonoBehaviour
{
    public TMP_Text descriptionHeart;
    public TMP_Text descriptionSpade;
    public TMP_Text descriptionSquare;
    public TMP_Text descriptionClover;
    public TMP_Text descriptionShield;
    public TMP_Text descriptionEnergy;
    public TMP_Text descriptionTurn;

    public GameObject maskedHeart;
    public GameObject maskedSpade;
    public GameObject maskedSquare;
    public GameObject maskedClover;
    public GameObject maskedShield;
    public GameObject maskedEnergy;
    public GameObject maskedTurn;

    public GameObject maskBackground;
    public GameObject panelTuto;

    public int tutoStep;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GameManager.currentEvent.tuto)
        {
            LauchTuto();
        }
    }

    private void LauchTuto()
    {

    }
}
