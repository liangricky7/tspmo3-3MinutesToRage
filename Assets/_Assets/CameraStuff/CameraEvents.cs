using System;

public static class CameraEvents
{
    public static Action<float, float> OnShake;

    public static void TriggerShake(float duration, float strength)
    {
        OnShake?.Invoke(duration, strength);
    }
}