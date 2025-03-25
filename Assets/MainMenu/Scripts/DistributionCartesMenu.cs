using UnityEngine;
using System.Collections;

public class DistributionCartes : MonoBehaviour
{
    public GameObject carteOptions;
    public GameObject carteJouer;
    public GameObject carteCredit;

    private Vector3[] positions = new Vector3[3]; // Positions cibles
    private GameObject[] cartes; // Tableau des cartes
    private Vector3[] startPositions; // Positions de départ (en haut de l'écran)
    private float speed = 1.5f; // Vitesse de l'animation (déplacement)
    private float delay = 0.3f; // Délai d'attente entre deux cartes distribuées

    public AudioSource audioSource;  // Composant AudioSource
    public AudioClip cardSound;      // Son de distribution des cartes

    void Start()
    {
        // Définir les positions finales des cartes (gauche, centre, droite)
        positions[0] = new Vector3(-175, -115, 0); // Gauche
        positions[1] = new Vector3(0, -115+40, 0);    // Centre
        positions[2] = new Vector3(175, -115, 0);  // Droite

        // Stocker les cartes dans un tableau
        cartes = new GameObject[] { carteOptions, carteJouer, carteCredit };

        // Stocker les positions de départ (en haut de l'écran)
        startPositions = new Vector3[3];
        for (int i = 0; i < startPositions.Length; i++)
        {
            startPositions[i] = new Vector3(0, 500, 0); // Position initiale en haut
            cartes[i].GetComponent<RectTransform>().anchoredPosition = startPositions[i];
        }

        // Démarrer la distribution des cartes une par une, avec un petit délai entre chaque carte
        for (int i = 0; i < cartes.Length; i++)
        {
            StartCoroutine(DistribuerCarte(i, i * delay)); // Délai entre chaque carte
        }
    }

    IEnumerator DistribuerCarte(int index, float delay)
    {
        // Attendre un délai avant de commencer le mouvement de cette carte
        yield return new WaitForSeconds(delay);

        float t = 0; // Réinitialiser t pour l'animation de chaque carte
        RectTransform rect = cartes[index].GetComponent<RectTransform>();

        // Rotation initiale (commencer droite)
        float startRotation = 0f;
        float endRotation = 360f; // Tour complet sur la carte (horaire)

        if (audioSource != null && cardSound != null)
        {
            audioSource.PlayOneShot(cardSound);
        }

        // Déplacer la carte vers sa position cible tout en effectuant une rotation continue dans le sens horaire
        while (t < 1)
        {
            t += Time.deltaTime * speed; // Progression de l'animation

            // Déplacer la carte vers sa position cible
            rect.anchoredPosition = Vector3.Lerp(startPositions[index], positions[index], t);

            // Appliquer une interpolation pour la rotation (rotation fluide dans le sens horaire)
            float currentRotation = Mathf.Lerp(startRotation, -endRotation, t); // Sens horaire (négatif)
            rect.rotation = Quaternion.Euler(0, 0, currentRotation); // Appliquer la 

            yield return null;
        }

        // Une fois la carte arrivée à destination, la remettre droite (0°)
        rect.rotation = Quaternion.Euler(0, 0, 0); // Arrive droite dans la main
    }

    void ShuffleArray(Vector3[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            Vector3 temp = array[i];
            int randomIndex = Random.Range(i, array.Length);
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }
}
