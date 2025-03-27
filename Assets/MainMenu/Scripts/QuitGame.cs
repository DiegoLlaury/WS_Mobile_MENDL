using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitGame : MonoBehaviour
{
    public GameObject quitPopup; // Assigne le panel de confirmation depuis l'éditeur
    public GameObject playPopup;

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
        // Désactiver la popup si l'utilisateur annule
        quitPopup.SetActive(false);
    }
    
    public void LaunchGame()
    {
       SceneManager.LoadScene("MacroScene");
       GameManager.isGameStarted = false;
    }

    public void LaunchTuto()
    {
        SceneManager.LoadScene("MacroSceneTuto");
        GameManager.isGameStarted = false;
    }
}
