using System.Collections;
using UnityEngine;

public class EffectBoardingTravel : Effect
{
    public float travelDuration;
    [SerializeField] private PixelPerfectSprite shuttleSprite;

    private Vector3 shuttleOrigin;
    private Vector3 shuttleTarget;

    private PixelPerfectSprite shuttle;

    public EffectBoardingTravel Setup(Vector3 origin, Vector3 target)
    {
        float dx = target.x - origin.x;
        float rx = shuttleSprite.Width * .5f;
        Vector3 offset = Vector3.right * (Mathf.Sign(dx) * rx);

        shuttleOrigin = origin + offset;
        shuttleTarget = target - offset;

        shuttle = Instantiate(shuttleSprite, transform);
        if (shuttleOrigin.x < shuttleTarget.x) shuttle.GetRenderer().flipX = true;

        Game.Instance.audioManager.Play("laser"); // TODO Pick a better sound
        return this;
    }

    protected override bool OnUpdate(float time)
    {
        float mu = time / travelDuration;
        UpdateShuttle(mu);
        return mu < 1f;
    }

    private void UpdateShuttle(float mu)
    {
        shuttle.transform.position = Vector3.Lerp(shuttleOrigin, shuttleTarget, mu);
    }

    protected override IEnumerator TickUntilDone()
    {
        yield return base.TickUntilDone();
        Destroy(shuttle.gameObject);
    }
}
