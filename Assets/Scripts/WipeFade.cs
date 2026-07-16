using UnityEngine;

// Attach to the bloody car's SpriteRenderer, layered directly on top of a
// clean car sprite at the same position/scale. Slices its own sprite into a
// grid of small tiles as children at startup - each tile fades independently
// based on how close the cursor gets to it, so wiping only clears the spot
// you actually drag over instead of the whole car fading at once.
[RequireComponent(typeof(SpriteRenderer))]
public class WipeFade : MonoBehaviour
{
    public int columns = 10;
    public int rows = 7;
    public float brushRadiusWorldUnits = 0.8f;

    SpriteRenderer[] tiles;
    float[] tileWiped; // 0 = fully bloody, 1 = fully wiped, per tile

    void Awake()
    {
        SpriteRenderer source = GetComponent<SpriteRenderer>();
        Sprite sprite = source.sprite;
        Texture2D texture = sprite.texture;
        Rect spriteRectPixels = sprite.textureRect;
        float ppu = sprite.pixelsPerUnit;

        float localWidth = spriteRectPixels.width / ppu;
        float localHeight = spriteRectPixels.height / ppu;
        float tileLocalWidth = localWidth / columns;
        float tileLocalHeight = localHeight / rows;
        float tilePixelWidth = spriteRectPixels.width / columns;
        float tilePixelHeight = spriteRectPixels.height / rows;

        tiles = new SpriteRenderer[columns * rows];
        tileWiped = new float[columns * rows];

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                int index = y * columns + x;

                GameObject tileObj = new GameObject($"Tile_{x}_{y}");
                tileObj.transform.SetParent(transform, false);
                tileObj.transform.localPosition = new Vector3(
                    -localWidth / 2f + (x + 0.5f) * tileLocalWidth,
                    -localHeight / 2f + (y + 1.1f) * tileLocalHeight,
                    0f);

                SpriteRenderer tileSr = tileObj.AddComponent<SpriteRenderer>();
                tileSr.sharedMaterial = source.sharedMaterial;
                tileSr.sortingLayerID = source.sortingLayerID;
                tileSr.sortingOrder = source.sortingOrder;

                Rect tileRect = new Rect(
                    spriteRectPixels.x + x * tilePixelWidth,
                    spriteRectPixels.y + y * tilePixelHeight,
                    tilePixelWidth, tilePixelHeight);
                tileSr.sprite = Sprite.Create(texture, tileRect, new Vector2(0.5f, 0.5f), ppu);

                tiles[index] = tileSr;
            }
        }

        source.enabled = false;
    }

    public void ResetStain()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            tileWiped[i] = 0f;
            ApplyAlpha(i);
        }
    }

    // Snaps every tile to fully clean, even ones that were never actually wiped.
    public void CompleteWipe()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            tileWiped[i] = 1f;
            ApplyAlpha(i);
        }
    }

    // amount is the wipe strength for this frame at the brush center;
    // tiles closer to worldPos get more of it, tiles outside the brush get none.
    public void WipeAtWorldPoint(Vector2 worldPos, float amount)
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            float dist = Vector2.Distance(worldPos, tiles[i].transform.position);
            if (dist > brushRadiusWorldUnits) continue;

            float falloff = 1f - Mathf.Clamp01(dist / brushRadiusWorldUnits);
            tileWiped[i] = Mathf.Clamp01(tileWiped[i] + amount * falloff);
            ApplyAlpha(i);
        }
    }

    void ApplyAlpha(int i)
    {
        Color c = tiles[i].color;
        c.a = 1f - tileWiped[i];
        tiles[i].color = c;
    }

    // Average fraction wiped across every tile.
    public float GetWipedFraction()
    {
        float sum = 0f;
        for (int i = 0; i < tileWiped.Length; i++) sum += tileWiped[i];
        return sum / tileWiped.Length;
    }
}
