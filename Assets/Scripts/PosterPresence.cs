using UnityEngine;

// Hides this poster on load if it was already ripped down in the mini-game.
public class PosterPresence : MonoBehaviour
{
    void Awake()
    {
        if (PosterState.Removed) gameObject.SetActive(false);
    }
}
