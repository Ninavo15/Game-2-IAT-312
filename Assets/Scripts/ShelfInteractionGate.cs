using UnityEngine;

// Once an item has been chosen in the Selection Object mini-game, the
// convenience store's shelf should no longer offer its interact prompt back
// in Scene 4.
[RequireComponent(typeof(Interactable))]
public class ShelfInteractionGate : MonoBehaviour
{
    void Awake()
    {
        bool itemChosen = GlobalStore.scissorPick || GlobalStore.bleachPick || GlobalStore.wrenchPick;
        if (itemChosen) GetComponent<Interactable>().enabled = false;
    }
}
