using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Wired up by CarInteractionGate once the car has been washed: pressing E on
// the car now tries to leave in it instead of restarting the wipe mini-game.
// Requires gas pumped and an item bought from the convenience store first -
// missing either blocks entry with an explanatory line instead.
public class CarEntryInteraction : MonoBehaviour
{
    public DialogueController dialogue;
    public GameObject player;
    public Fading fade;
    public CarMovement carMovement;

    [Header("Missing requirement lines")]
    public string needBothLine = "I need to pump some gas and buy some stuffs";
    public string needGasLine = "I need some gas";
    public string needItemLine = "I need to buy some stuffs from convenience store";

    [Header("Drive away")]
    public string transitionScene = "Transition SIde";
    public float driveAwaySpeed = 8f;
    public float driveAwayDuration = 2f;

    Rigidbody2D rb;
    bool leaving;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Wired to the car's Interactable.onInteract by CarInteractionGate.
    public void TryEnterCar()
    {
        if (leaving) return;

        bool hasGas = GasPumpState.Attempted;
        bool hasItem = GlobalStore.bleachPick || GlobalStore.scissorPick || GlobalStore.wrenchPick;

        if (!hasGas && !hasItem)
        {
            dialogue.ShowLine(needBothLine);
            return;
        }
        if (!hasGas)
        {
            dialogue.ShowLine(needGasLine);
            return;
        }
        if (!hasItem)
        {
            dialogue.ShowLine(needItemLine);
            return;
        }

        leaving = true;
        StartCoroutine(DriveAwaySequence());
    }

    IEnumerator DriveAwaySequence()
    {
        if (player != null) player.SetActive(false);
        if (carMovement != null) carMovement.enabled = false;

        // Start fading to black as soon as the car starts moving, not after
        // it stops - the drive and the fade run at the same time.
        fade.FadeOut();

        float elapsed = 0f;
        while (elapsed < driveAwayDuration)
        {
            elapsed += Time.fixedDeltaTime;
            rb.MovePosition(rb.position + Vector2.right * driveAwaySpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }

        // If the fade takes longer than the drive, wait out the rest of it
        // before loading so the screen is fully black first.
        float remainingFadeTime = fade.fadeDuration - driveAwayDuration;
        if (remainingFadeTime > 0f) yield return new WaitForSeconds(remainingFadeTime);

        SceneManager.LoadScene(transitionScene);
    }
}
