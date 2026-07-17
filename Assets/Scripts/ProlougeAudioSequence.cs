using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// This scene has no visuals of its own yet - it's just a sound-only beat
// before gameplay starts: the car speeding, then crashing, then footsteps,
// then the trunk closing, before loading into Scene 1.
public class ProlougeAudioSequence : MonoBehaviour
{
    public AudioSource audioSource;

    [Header("Clips")]
    public AudioClip carSpeedClip;
    public float carSpeedDuration = 5f; // cut short here even if the clip runs longer
    public AudioClip carCrashClip;
    public AudioClip carDoorOpenClip;
    public AudioClip walkingClip;
    public AudioClip trunkCloseClip;

    [Header("Next scene")]
    public string nextScene = "Scene 1";

    void Start()
    {
        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        yield return PlayAndWait(carSpeedClip, carSpeedDuration);
        yield return PlayAndWait(carCrashClip);
        yield return PlayAndWait(carDoorOpenClip);
        yield return PlayAndWait(walkingClip);
        yield return PlayAndWait(trunkCloseClip);

        SceneManager.LoadScene(nextScene);
    }

    IEnumerator PlayAndWait(AudioClip clip)
    {
        if (clip == null) yield break;
        yield return PlayAndWait(clip, clip.length);
    }

    IEnumerator PlayAndWait(AudioClip clip, float duration)
    {
        if (clip != null) audioSource.PlayOneShot(clip);
        yield return new WaitForSeconds(duration);
    }
}
