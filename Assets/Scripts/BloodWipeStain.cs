using UnityEngine;

// Attach to the blood-covered SpriteRenderer (e.g. "car bloody side").
// Lets a brush erase alpha locally as the player wipes, and can snap the
// stain back to fully bloody with ResetStain() - the wipe never sticks.
[RequireComponent(typeof(SpriteRenderer))]
public class BloodWipeStain : MonoBehaviour
{
    [Header("Brush")]
    public float brushRadiusPixels = 14f;
    [Range(0f, 1f)] public float brushSoftness = 0.5f;

    SpriteRenderer sr;
    Texture2D workingTexture;
    Sprite workingSprite;
    Color32[] originalPixels;
    Color32[] currentPixels;
    int texWidth, texHeight;
    long totalAlphaSum;
    long remainingAlphaSum;
    bool ready;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        BuildReadableCopy();
        ResetStain();
    }

    void BuildReadableCopy()
    {
        Texture2D source = sr.sprite.texture;

        if (!source.isReadable)
        {
            Debug.LogError($"BloodWipeStain: '{source.name}' needs Read/Write Enabled checked in its Import Settings (select the texture in Project, tick it in the Inspector, then Apply).", this);
            return;
        }

        Rect spriteRectPixels = sr.sprite.textureRect;
        texWidth = Mathf.RoundToInt(spriteRectPixels.width);
        texHeight = Mathf.RoundToInt(spriteRectPixels.height);

        originalPixels = CropPixels32(source, Mathf.RoundToInt(spriteRectPixels.x), Mathf.RoundToInt(spriteRectPixels.y), texWidth, texHeight);

        currentPixels = new Color32[originalPixels.Length];

        workingTexture = new Texture2D(texWidth, texHeight, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = source.filterMode
        };

        workingSprite = Sprite.Create(
            workingTexture,
            new Rect(0, 0, texWidth, texHeight),
            new Vector2(sr.sprite.pivot.x / texWidth, sr.sprite.pivot.y / texHeight),
            sr.sprite.pixelsPerUnit);

        sr.sprite = workingSprite;

        totalAlphaSum = 0;
        for (int i = 0; i < originalPixels.Length; i++) totalAlphaSum += originalPixels[i].a;
        if (totalAlphaSum <= 0) totalAlphaSum = 1; // guard against divide-by-zero

        ready = true;
    }

    static Color32[] CropPixels32(Texture2D fullTexture, int x, int y, int width, int height)
    {
        Color32[] full = fullTexture.GetPixels32();
        int fullWidth = fullTexture.width;
        Color32[] cropped = new Color32[width * height];

        for (int row = 0; row < height; row++)
        {
            System.Array.Copy(full, (y + row) * fullWidth + x, cropped, row * width, width);
        }

        return cropped;
    }

    // Restores the stain to fully bloody. Call at the start of every round.
    public void ResetStain()
    {
        if (!ready) return;

        System.Array.Copy(originalPixels, currentPixels, originalPixels.Length);
        workingTexture.SetPixels32(currentPixels);
        workingTexture.Apply(false);
        remainingAlphaSum = totalAlphaSum;
    }

    public void WipeAtWorldPoint(Vector2 worldPos)
    {
        if (!ready) return;

        Vector3 local = transform.InverseTransformPoint(worldPos);
        Bounds b = sr.sprite.bounds;

        float u = (local.x - b.min.x) / b.size.x;
        float v = (local.y - b.min.y) / b.size.y;
        if (u < 0f || u > 1f || v < 0f || v > 1f) return;

        int centerX = Mathf.RoundToInt(u * texWidth);
        int centerY = Mathf.RoundToInt(v * texHeight);
        int r = Mathf.Max(1, Mathf.RoundToInt(brushRadiusPixels));

        int minX = Mathf.Max(0, centerX - r);
        int maxX = Mathf.Min(texWidth - 1, centerX + r);
        int minY = Mathf.Max(0, centerY - r);
        int maxY = Mathf.Min(texHeight - 1, centerY + r);
        if (minX > maxX || minY > maxY) return;

        int w = maxX - minX + 1;
        int h = maxY - minY + 1;
        Color32[] sub = new Color32[w * h];

        for (int y = 0; y < h; y++)
        {
            int py = minY + y;
            for (int x = 0; x < w; x++)
            {
                int px = minX + x;
                float dist = Vector2.Distance(new Vector2(px, py), new Vector2(centerX, centerY));
                int idx = py * texWidth + px;

                if (dist <= r)
                {
                    float falloff = 1f - Mathf.Clamp01((dist / r - brushSoftness) / Mathf.Max(0.001f, 1f - brushSoftness));
                    falloff = Mathf.Clamp01(falloff);

                    Color32 c = currentPixels[idx];
                    byte newAlpha = (byte)Mathf.Min(c.a, Mathf.RoundToInt((1f - falloff) * originalPixels[idx].a));
                    if (newAlpha != c.a)
                    {
                        remainingAlphaSum -= (c.a - newAlpha);
                        currentPixels[idx] = new Color32(c.r, c.g, c.b, newAlpha);
                    }
                }
                sub[y * w + x] = currentPixels[idx];
            }
        }

        workingTexture.SetPixels32(minX, minY, w, h, sub);
        workingTexture.Apply(false);
    }

    // 0 = still fully bloody, 1 = every drop of blood wiped away.
    public float GetWipedFraction()
    {
        if (!ready) return 0f;
        return 1f - (float)remainingAlphaSum / totalAlphaSum;
    }
}
