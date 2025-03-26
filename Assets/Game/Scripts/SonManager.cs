using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;  // Singleton pour accéder facilement au son
    private AudioSource audioSource;

    void Awake()
    {
        // Singleton : assure qu'un seul AudioManager existe
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Garde l'objet actif entre les scènes
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    // Fonction pour jouer un son
    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
