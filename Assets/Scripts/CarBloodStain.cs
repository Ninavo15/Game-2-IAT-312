using UnityEngine;

// Swaps this sprite to the clean version if the player successfully washed
// the car in the Wipe Mini Game. Stays bloody otherwise.
[RequireComponent(typeof(SpriteRenderer))]
public class CarBloodStain : MonoBehaviour
{
    public Sprite cleanSprite;

    void Awake()
    {
        if (!CarWashState.Cleaned) return;
        if (cleanSprite == null) return;
        GetComponent<SpriteRenderer>().sprite = cleanSprite;
    }
}
