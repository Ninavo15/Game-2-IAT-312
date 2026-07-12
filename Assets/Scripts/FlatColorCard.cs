using UnityEngine;

// Builds a solid-color sprite once, since there's no actual flat-color image
// asset to assign. Position, scale, and sorting are entirely up to whatever
// you set on this GameObject's Transform / Sprite Renderer in the Editor -
// this script never touches them, so nothing moves or recolors on its own
// when you enter Play mode.
[RequireComponent(typeof(SpriteRenderer))]
public class FlatColorCard : MonoBehaviour
{
    public Color cardColor = new Color(105f / 255f, 183f / 255f, 219f / 255f);

    void Awake()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply(false);
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        sr.color = cardColor;
    }
}
