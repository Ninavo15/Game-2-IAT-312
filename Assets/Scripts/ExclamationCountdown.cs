using UnityEngine;

// Shows "[!]" in red above the worker for the moment they actually notice
// the player mid-rip. Hidden the rest of the time - the worker can turn to
// check without ever triggering this if the player isn't caught ripping.
[RequireComponent(typeof(TextMesh))]
public class ExclamationCountdown : MonoBehaviour
{
    public Color dangerColor = Color.red;

    TextMesh textMesh;

    void Awake()
    {
        textMesh = GetComponent<TextMesh>();
        gameObject.SetActive(false);
    }

    public void ShowDanger()
    {
        gameObject.SetActive(true);
        textMesh.color = dangerColor;
        textMesh.text = "[!]";
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
