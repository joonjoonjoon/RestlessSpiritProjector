using UnityEngine;

public static class TimeHelper
{
    public static string GetTimeString(float time)
    {
        System.TimeSpan t = System.TimeSpan.FromSeconds(time);

        return string.Format("{0:D2}:{1:D2}<size=75%>.{2:D3}</size>", (int)t.TotalMinutes, t.Seconds, t.Milliseconds);
    }
}
