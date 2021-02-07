using UnityEngine;

public class EffectEvade : Effect
{
    public float duration;
    public float magnitude;
    private VesselVisuals vessel;
    private Transform[] targets;
    private Vector3[] origins;

    public EffectEvade Setup(VesselVisuals vessel)
    {
        this.vessel = vessel;

        targets = new Transform[]
        {
            vessel.hull.transform,
            vessel.shield.transform,
            vessel.engineTrail.transform
        };

        origins = new Vector3[targets.Length];
        for (int i = 0; i < targets.Length; ++i)
            origins[i] = targets[i].localPosition;

        UpdateTransforms(0);
        return this;
    }

    private void UpdateTransforms(float mu)
    {
        mu = Mathf.Clamp(mu, 0f, 1f);
        float offset = Mathf.Sin(mu * Mathf.PI) * magnitude;

        for (int i = 0; i < targets.Length; ++i)
            targets[i].localPosition = origins[i] + new Vector3(0, offset, 0);
    }

    protected override bool OnUpdate(float time)
    {
        float mu = time / duration;
        UpdateTransforms(mu);
        return mu < 1f;
    }
}
