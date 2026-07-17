using UnityEngine;

public static class StressSystem
{
    public static int stressPoints = 0;

    public static void AddPoint(int amount = 1)
    {
        Debug.Log(stressPoints);
        stressPoints += amount;
    }

    public static void Reset()
    {
        stressPoints = 0;
    }
}
