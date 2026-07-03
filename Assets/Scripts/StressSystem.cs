using UnityEngine;

public static class StressSystem
{
    public static int stressPoints = 0;

    public static void AddPoint(int amount = 1)
    {
        stressPoints += amount;
    }

    public static void Reset()
    {
        stressPoints = 0;
    }
}
