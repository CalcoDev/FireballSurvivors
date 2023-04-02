using Godot;

namespace Game.Utils;

public static class Calc
{
    public static bool SameSign(float a, float b)
    {
        return (a < 0) == (b < 0);
    }

    public static bool SameSignZero(float a, float b)
    {
        if (a == 0 || b == 0)
            return true;

        return (a < 0) == (b < 0);
    }

    public static float Approach(float current, float target, float maxDelta)
    {
        return current < target ? Mathf.Min(current + maxDelta, target) : Mathf.Max(current - maxDelta, target);
    }

    public static bool FloatEquals(float a, float b, float epsilon = 0.001f)
    {
        return Mathf.Abs(a - b) < epsilon;
    }

    public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
    {
        smoothTime = Mathf.Max(0.0001f, smoothTime);
        float num = 2f / smoothTime;
        float num2 = num * deltaTime;
        float num3 = 1f / (1f + num2 + (0.48f * num2 * num2) + (0.235f * num2 * num2 * num2));
        float num4 = current - target;
        float num5 = target;
        float num6 = maxSpeed * smoothTime;
        num4 = Mathf.Clamp(num4, -num6, num6);
        target = current - num4;
        float num7 = (currentVelocity + (num * num4)) * deltaTime;
        currentVelocity = (currentVelocity - (num * num7)) * num3;
        float num8 = target + ((num4 + num7) * num3);
        if (num5 - current > 0f == num8 > num5)
        {
            num8 = num5;
            currentVelocity = (num8 - num5) / deltaTime;
        }
        return num8;
    }

    public static float MapRange(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return outMin + (outMax - outMin) * ((value - inMin) / (inMax - inMin));
    }
}