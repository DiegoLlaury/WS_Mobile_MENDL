using UnityEngine;
using WS_DiegoCo_Event;

public class MusicManager : MonoBehaviour
{
    public AudioSource music1;
    public AudioSource music2;
    public AudioSource music3;

    void Start()
    {
        PlayMusic(1); // Joue la première musique par défaut
    }

    public void PlayMusic(int trackNumber)
    {
        // Désactive toutes les musiques
        music1.Stop();
        music2.Stop();
        music3.Stop();

        // Active la musique choisie
        switch (GameManager.currentEvent.eventType)
        {
            case EventBattle.EventType.Combat:
                music1.Play();
                break;

            case EventBattle.EventType.Infiltration:
                music3.Play();
                break;

            case EventBattle.EventType.Enquete:
                music2.Play();
                break;
        }
    }
}
