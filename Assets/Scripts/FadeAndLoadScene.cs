using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Fades to black and loads another scene, with no dialogue line in between -
// for interactions that should just transition straight through (e.g.
// walking through a door), unlike SceneTransitionAfterDialogue.
public class FadeAndLoadScene : MonoBehaviour
{
    public Fading fade;
    public string sceneToLoad;

    public void Trigger()
    {
        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        fade.FadeOut();
        yield return new WaitForSeconds(fade.fadeDuration);
        SceneManager.LoadScene(sceneToLoad);
    }
}
