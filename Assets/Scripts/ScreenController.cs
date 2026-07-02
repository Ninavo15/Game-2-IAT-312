using UnityEngine;

public class ScreenController : MonoBehaviour
{
    enum State { Off, MissingScreen, Footage }

    public SpriteRenderer screenRenderer;
    public Sprite missingScreenSprite;
    public Sprite footageSprite;

    State state = State.Off;

    public void TurnOnAndShowMissing()
    {
        if (state != State.Off) return;
        state = State.MissingScreen;
        screenRenderer.sprite = missingScreenSprite;
    }

    public void ShowFootage()
    {
        if (state != State.MissingScreen) return;
        state = State.Footage;
        screenRenderer.sprite = footageSprite;
    }
}
