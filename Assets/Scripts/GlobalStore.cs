using UnityEngine;

public static class GlobalStore
{
    public static bool scissorPick = false;
    public static bool bleachPick = true;
    public static bool wrenchPick = false;
    public static bool bloodWiped = false;

    // Remembers where the player was standing in Scene 4 before entering the
    // convenience store's item-selection mini-game (Selection Object scene),
    // so Scene 4 can drop them back at the same spot instead of resetting to
    // the scene's default start position.
    public static bool returnedFromSelection = false;
    public static Vector2 selectionReturnPosition;
}
