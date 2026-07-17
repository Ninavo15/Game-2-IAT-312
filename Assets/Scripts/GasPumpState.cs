using UnityEngine;

// Persists how much fuel was pumped in the Gas Pump mini-game across the
// scene switch back to Scene 3, so later scenes/scripts can reference how
// full the tank is.
public static class GasPumpState
{
    public static float FuelPumped;

    // True once the mini-game has been played at all (win or time-up).
    public static bool Attempted;
}
