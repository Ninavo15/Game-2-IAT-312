using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

// Once the wipe mini-game has been played (win or lose), the car's interact
// prompt switches from "check the car" (which restarts the wipe mini-game)
// to "enter the car" (drive off - see CarEntryInteraction), rather than
// simply disabling interaction like the other one-shot gates.
[RequireComponent(typeof(Interactable))]
public class CarInteractionGate : MonoBehaviour
{
    public Text promptLabel;
    public string enterCarPromptText = "Press E to Enter the Car";
    public CarEntryInteraction carEntry;

    void Awake()
    {
        if (!CarWashState.Played) return;

        Interactable interactable = GetComponent<Interactable>();
        interactable.onInteract = new UnityEvent();
        interactable.onInteract.AddListener(carEntry.TryEnterCar);

        // TryEnterCar can be a blocked no-op (missing gas/item) - the player
        // needs to be able to keep pressing E until both conditions are met.
        interactable.oneShot = false;

        if (promptLabel != null) promptLabel.text = enterCarPromptText;
    }
}
