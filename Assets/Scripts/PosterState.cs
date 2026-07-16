using UnityEngine;

// Persists whether the missing poster was successfully ripped down, and
// where the player was standing in Scene 3, across the scene switch to and
// from the Ripping poster mini-game.
public static class PosterState
{
    public static bool Removed;
    public static bool ReturnedFromMiniGame;
    public static Vector2 ReturnPosition;

    // True once the ripping mini-game has been played at all (win or lose) -
    // used to stop offering the poster's interact prompt again afterward.
    public static bool Attempted;
}
