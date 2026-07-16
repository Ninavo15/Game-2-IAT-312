// Persists whether the player successfully wiped the car clean in the Wipe
// Mini Game, across the scene switch back to Scene 3.
public static class CarWashState
{
    public static bool Cleaned;

    // True once the wipe mini-game has been played at all (win or lose) -
    // used to stop offering the car's interact prompt again afterward.
    public static bool Played;
}
