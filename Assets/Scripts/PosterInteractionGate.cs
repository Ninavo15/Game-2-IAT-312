using UnityEngine;

// Once the ripping mini-game has been played (win or lose), the poster
// should no longer offer its interact prompt when the player is nearby.
[RequireComponent(typeof(Interactable))]
public class PosterInteractionGate : MonoBehaviour
{
    void Awake()
    {
        if (PosterState.Attempted) GetComponent<Interactable>().enabled = false;
    }
}
