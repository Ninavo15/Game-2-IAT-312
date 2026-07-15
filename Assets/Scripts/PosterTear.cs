using UnityEngine;

// Slices the poster sprite into a grid of small pieces at startup. As rip
// progress increases, pieces disappear one at a time in a shuffled order, so
// the poster visibly tears away bit by bit instead of just fading uniformly.
[RequireComponent(typeof(SpriteRenderer))]
public class PosterTear : MonoBehaviour
{
    public int columns = 4;
    public int rows = 3;

    SpriteRenderer[] pieces;
    int[] removalOrder;
    int removedCount;

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

        // sprite.pivot is in pixels relative to the sprite's own rect.
        float pivotOffsetX = sprite.pivot.x / ppu;
        float pivotOffsetY = sprite.pivot.y / ppu;

        int total = columns * rows;
        pieces = new SpriteRenderer[total];

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                int index = y * columns + x;

                GameObject pieceObj = new GameObject($"Piece_{x}_{y}");
                pieceObj.transform.SetParent(transform, false);
                pieceObj.transform.localPosition = new Vector3(
                    (x + 0.5f) * tileLocalWidth - pivotOffsetX,
                    (y + 0.5f) * tileLocalHeight - pivotOffsetY,
                    0f);

                SpriteRenderer pieceSr = pieceObj.AddComponent<SpriteRenderer>();
                pieceSr.sharedMaterial = source.sharedMaterial;
                pieceSr.sortingLayerID = source.sortingLayerID;
                pieceSr.sortingOrder = source.sortingOrder;

                Rect pieceRect = new Rect(
                    spriteRectPixels.x + x * tilePixelWidth,
                    spriteRectPixels.y + y * tilePixelHeight,
                    tilePixelWidth, tilePixelHeight);
                pieceSr.sprite = Sprite.Create(texture, pieceRect, new Vector2(0.5f, 0.5f), ppu);

                pieces[index] = pieceSr;
            }
        }

        source.enabled = false;

        removalOrder = new int[total];
        for (int i = 0; i < total; i++) removalOrder[i] = i;
        for (int i = total - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (removalOrder[i], removalOrder[j]) = (removalOrder[j], removalOrder[i]);
        }
    }

    public void ResetPoster()
    {
        foreach (SpriteRenderer piece in pieces) piece.enabled = true;
        removedCount = 0;
    }

    // 0 = fully intact, 1 = fully torn down.
    public void SetProgress(float progress)
    {
        int targetRemoved = Mathf.FloorToInt(Mathf.Clamp01(progress) * pieces.Length);
        while (removedCount < targetRemoved)
        {
            pieces[removalOrder[removedCount]].enabled = false;
            removedCount++;
        }
    }
}
