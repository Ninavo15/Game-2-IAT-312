using UnityEngine;

// Once the gas pump mini-game has been played (win or lose), the pump should
// no longer offer its interact prompt when the player is standing next to it.
[RequireComponent(typeof(Interactable))]
public class GasPumpInteractionGate : MonoBehaviour
{
    void Awake()
    {
        if (GasPumpState.Attempted) GetComponent<Interactable>().enabled = false;
    }
}
