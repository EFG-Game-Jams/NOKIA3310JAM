using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Distribution
{
    public static float Uniform()
    {
        return Random.value;
    }

    public static float Uniform(float min, float max)
    {
        return Random.Range(min, max);
    }

    public static float LinearBias(float gradient)
    {
        return Mathf.Sqrt(Mathf.Pow(Uniform(), 2f - gradient));
    }

    public static float CurvedBias(float offset, float curve)
    {
        float exponent = Mathf.Max(offset, 1f - (curve * (1f - offset)));
        return Mathf.Pow(Uniform(), exponent);
    }
}
