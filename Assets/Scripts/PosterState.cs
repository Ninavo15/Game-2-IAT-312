using UnityEngine;

// Persists whether the missing poster was successfully ripped down, and
// where the player was standing in Scene 3, across the scene switch to and
// from the Ripping poster mini-game.
public static class PosterState
{
    public static bool Removed;
    public static bool ReturnedFromMiniGame;
    public static Vector2 ReturnPosition;
}
