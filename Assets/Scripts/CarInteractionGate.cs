using UnityEngine;

// Once the wipe mini-game has been played (win or lose), the car should no
// longer offer its interact prompt when the player is standing next to it.
[RequireComponent(typeof(Interactable))]
public class CarInteractionGate : MonoBehaviour
{
    void Awake()
    {
        if (CarWashState.Played) GetComponent<Interactable>().enabled = false;
    }
}
