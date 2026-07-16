using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public GameObject promptText;
    public UnityEvent onInteract;

    bool playerInRange = false;

    void Awake()
    {
        if (promptText != null) promptText.SetActive(false);
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
