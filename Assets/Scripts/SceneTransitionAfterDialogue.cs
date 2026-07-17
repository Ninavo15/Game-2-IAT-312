using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Shows a dialogue line for a fixed total duration (reveal + hold combined),
// then automatically fades to black and loads another scene - no extra
// input needed from the player.
public class SceneTransitionAfterDialogue : MonoBehaviour
{
    public DialogueController dialogue;
    public Fading fade;
    public string sceneToLoad;
    public float totalDisplayDuration = 5f;

    public void ShowThenLoad(string line)
    {
        StartCoroutine(Sequence(line));
    }

    IEnumerator Sequence(string line)
    {
        // dialogue is optional - scenes with no dialogue box just fade straight through.
        if (dialogue != null)
        {
            float revealTime = line.Length / Mathf.Max(dialogue.charactersPerSecond, 1f);
            float holdTime = Mathf.Max(0f, totalDisplayDuration - revealTime);
            dialogue.ShowLine(line, holdTime);

            yield return new WaitForSeconds(totalDisplayDuration);
        }

        fade.FadeOut();
        yield return new WaitForSeconds(fade.fadeDuration);

        // No destination decided yet - reload the current scene rather than
        // erroring out on an empty scene name.
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
