using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public GameObject promptText;
    public UnityEvent onInteract;

    [Tooltip("Show the prompt immediately if the player is returning from a mini-game (e.g. the convenience store's item-selection scene) - a teleport straight back into this trigger doesn't reliably fire a trigger event on the same frame.")]
    public bool showPromptIfReturnedFromSelection = false;

    bool playerInRange = false;

    void Awake()
    {
        if (promptText != null) promptText.SetActive(false);
        if (showPromptIfReturnedFromSelection && GlobalStore.returnedFromSelection) playerInRange = true;
    }

    void OnDisable()
    {
        playerInRange = false;
        if (promptText != null) promptText.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<CharacterMovement>() == null) return;
        playerInRange = true;
    }

    // Catches the case where the player is teleported directly into an
    // already-overlapping trigger (e.g. returning from a mini-game) - a
    // teleport like that doesn't always fire OnTriggerEnter2D.
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<CharacterMovement>() == null) return;
        playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<CharacterMovement>() == null) return;
        playerInRange = false;
        if (promptText != null) promptText.SetActive(false);
    }

    void Update()
    {
        bool showPrompt = playerInRange && !DialogueController.IsActive;
        if (promptText != null && promptText.activeSelf != showPrompt)
        {
            promptText.SetActive(showPrompt);
        }

        if (showPrompt && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (promptText != null) promptText.SetActive(false);
            enabled = false; // one-shot: don't let the prompt flicker back on afterward
            onInteract.Invoke();
        }
    }
}
