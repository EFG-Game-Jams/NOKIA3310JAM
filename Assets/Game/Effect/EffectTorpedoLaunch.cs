using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTorpedoLaunch : Effect
{
    public float travelDuration;
    [SerializeField] private PixelPerfectSprite torpedoSprite;

    private Vector3 torpedoOrigin;
    private Vector3 torpedoTarget;

    private PixelPerfectSprite torpedo;

    public EffectTorpedoLaunch Setup(Vector3 origin, Vector3 target)
    {
        float dx = target.x - origin.x;
        float rx = torpedoSprite.Width * .5f;
        Vector3 offset = Vector3.right * (Mathf.Sign(dx) * rx);

        torpedoOrigin = origin + offset;
        torpedoTarget = target - offset;

        torpedo = Instantiate(torpedoSprite, transform);
        if (torpedoOrigin.x < torpedoTarget.x) torpedo.GetRenderer().flipX = true;

        Game.Instance.audioManager.Play("laser"); // TODO Pick a better sound
        return this;
    }

    protected override bool OnUpdate(float time)
    {
        float mu = time / travelDuration;
        UpdateTorpedo(mu);
        return mu < 1f;
    }

    private void UpdateTorpedo(float mu)
    {
        torpedo.transform.position = Vector3.Lerp(torpedoOrigin, torpedoTarget, mu);
    }

    protected override IEnumerator TickUntilDone()
    {
        yield return base.TickUntilDone();
        Destroy(torpedo.gameObject);
    }
}
