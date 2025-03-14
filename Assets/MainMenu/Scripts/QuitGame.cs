using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public GameObject quitPopup; // Assigne le panel de confirmation depuis l'�diteur

    public void Quitter()
    {
        // Activer la popup au lieu de quitter directement
        quitPopup.SetActive(true);
    }

    public void ConfirmQuit()
    {
        Debug.Log("Quitter l'application...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void CancelQuit()
    {
        // D�sactiver la popup si l'utilisateur annule
        quitPopup.SetActive(false);
    }
}
