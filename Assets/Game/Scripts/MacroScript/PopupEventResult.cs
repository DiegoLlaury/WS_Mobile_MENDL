using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupEventResult : MonoBehaviour
{
    public TMP_Text resultText;
    public Image backgroundImage;
    private CanvasGroup canvasGroup;

    public void SetupPopup(string resultMessage, bool isVictory)
    {
        resultText.text = resultMessage;
        resultText.color = isVictory ? Color.green : Color.red;
        canvasGroup = GetComponent<CanvasGroup>();

        // Animation d’apparition
        StartCoroutine(ShowPopup());
    }

    private IEnumerator ShowPopup()
    {
        float fadeDuration = 0.3f;
        float displayDuration = 7.5f;
        float elapsed = 0f;

        // Apparition progressive
        canvasGroup.alpha = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }

        // Attendre pendant 5 secondes
        yield return new WaitForSeconds(displayDuration);

        // Disparition progressive
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        // Suppression et réorganisation du Grid
        Destroy(gameObject);
        HUDMacro.Instance.RefreshGrid();
    }
}
