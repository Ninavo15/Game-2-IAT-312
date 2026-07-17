// Persists whether the player successfully wiped the car clean in the Wipe
// Mini Game, across the scene switch back to Scene 3.
public static class CarWashState
{
    public static bool Cleaned;

    // True once the wipe mini-game has been played at all (win or lose) -
    // used by CarInteractionGate to swap the car's interact prompt from
    // "check the car" to "enter the car" instead of offering it again.
    public static bool Played;
}
