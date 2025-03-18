using UnityEngine;
using UnityEngine.UI;
using WS_DiegoCo_Middle;
using TMPro;

public class CardMiddleDisplay : MonoBehaviour
{
    public CardMiddle cardData;
    
    public Image cardImage;
    public TMP_Text nameCard;
    public TMP_Text heartText;
    public TMP_Text squareText;
    public TMP_Text spadeText;
    public TMP_Text cloverText;
    public Image[] typeImages;

    private void Start()
    {
        UpdateCardMiddle();
    }

    private void UpdateCardMiddle()
    {
        heartText.text = cardData.heart.ToString();
        squareText.text = cardData.square.ToString();
        spadeText.text = cardData.spade.ToString();
        cloverText.text = cardData.clover.ToString();
    }
}
