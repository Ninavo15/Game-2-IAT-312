using UnityEngine;

// Persists the car's actual resting position from its one-time intro-drive
// sequence in Scene 3, across every later scene reload for the rest of the
// play session - so returning from any mini-game (poster, gas pump, wipe)
// always parks the car at the same spot, instead of snapping back to the
// hand-tuned autoDriveTarget baked into the scene (which doesn't exactly
// match where the timed intro drive actually stops).
public static class CarParkState
{
    public static bool Established;
    public static Vector2 Position;
}
