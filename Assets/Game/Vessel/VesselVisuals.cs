using UnityEngine;

public class VesselVisuals : MonoBehaviour
{
    public SpriteRenderer hull;
    public SpriteRenderer shield;
    public SpriteRenderer engineTrail;

    public Transform laserEmit;
    public Transform laserReceiveHull;
    public Transform laserReceiveShield;

    public Transform torpedoEmit;

    public bool HullVisible { get => hull.enabled; set => hull.enabled = value; }
    public bool ShieldVisible { get => shield.enabled; set => shield.enabled = value; }
    public bool TrailVisible { get => engineTrail.enabled; set => engineTrail.enabled = value; }

    public void ResetVisibility()
    {
        HullVisible = true;
        ShieldVisible = false;
        TrailVisible = false;
    }
}
