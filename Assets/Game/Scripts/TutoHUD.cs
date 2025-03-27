using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutoHUD : MonoBehaviour
{
    public Image heartImage;
    public Image spadeImage;
    public Image squareImage;
    public Image cloverImage;
    public Image shieldImage;
    public Image energyImage;
    public Image turnImage;
    public Image statusImage;
    public Image intentionImage;
    

    public GameObject maskedHeart;
    public GameObject maskedSpade;
    public GameObject maskedSquare;
    public GameObject maskedClover;
    public GameObject maskedShield;
    public GameObject maskedEnergy;
    public GameObject maskedTurn;
    //public GameObject maskedStatus;
    //public GameObject maskedIntention;

    public GameObject maskBackground;
    public GameObject panelTuto;
    public GameObject canvaTuto;

    public TMP_Text tutoStepText;
    public TMP_Text buttonText;

    public int tutoStep = 1;
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
        tutoStep = 1;
        canvaTuto.SetActive(true);
        maskBackground.SetActive(true);
        panelTuto.SetActive(true);
        buttonText.text = new string("Suivant");
        tutoStepText.text = $"{tutoStep.ToString()} / 9";
        PassTuto();
    }

    public void PassTuto()
    {
        switch (tutoStep)
        {

            case 1:
                maskedHeart.SetActive(true);
                heartImage.gameObject.SetActive(true);
                tutoStep++;
                break;

            case 2:
                tutoStepText.text = $"{tutoStep.ToString()} / 9";
                maskedSpade.SetActive(true);
                spadeImage.gameObject.SetActive(true);
                maskedHeart.SetActive(false);
                heartImage.gameObject.SetActive(false);
                tutoStep++;
                break;

            case 3:
                tutoStepText.text = $"{tutoStep.ToString()} / 9";
                maskedSquare.SetActive(true);
                squareImage.gameObject.SetActive(true);
                maskedSpade.SetActive(false);
                spadeImage.gameObject.SetActive(false);
                tutoStep++;
                break;

            case 4:
                tutoStepText.text = $"{tutoStep.ToString()} / 9";
                maskedClover.SetActive(true);
                cloverImage.gameObject.SetActive(true);
                maskedSquare.SetActive(false);
                squareImage.gameObject.SetActive(false);
                tutoStep++;
                break;

            case 5:
                tutoStepText.text = $"{tutoStep.ToString()} / 9";
                maskedShield.SetActive(true);
                shieldImage.gameObject.SetActive(true);
                maskedClover.SetActive(false);
                cloverImage.gameObject.SetActive(false);
                tutoStep++;
                break;

            case 6:
                tutoStepText.text = $"{tutoStep.ToString()} / 9";
                maskedEnergy.SetActive(true);
                energyImage.gameObject.SetActive(true);
                maskedShield.SetActive(false);
                shieldImage.gameObject.SetActive(false);
                tutoStep++;
                break;

            case 7:
                tutoStepText.text = $"{tutoStep.ToString()} / 9";
                maskedTurn.SetActive(true);
                turnImage.gameObject.SetActive(true);
                maskedEnergy.SetActive(false);
                energyImage.gameObject.SetActive(false);
                tutoStep++;
                break;

            case 8:
                tutoStepText.text = $"{tutoStep.ToString()} / 9";
                //maskedStatus.SetActive(true);
                statusImage.gameObject.SetActive(true);
                maskedTurn.SetActive(false);
                turnImage.gameObject.SetActive(false);
                tutoStep++;
                break;

            case 9:
                tutoStepText.text = $"{tutoStep.ToString()} / 9";
                buttonText.text = new string("Terminer");
                //maskedIntention.SetActive(true);
                intentionImage.gameObject.SetActive(true);
                //maskedStatus.SetActive(false);
                statusImage.gameObject.SetActive(false);
                tutoStep++;
                break;

            case 10:
                canvaTuto.SetActive(false);
                maskBackground.SetActive(false);
                panelTuto.SetActive(false);
               // maskedIntention.SetActive(false);
                intentionImage.gameObject.SetActive(false);
                break;
        }
    }
}
