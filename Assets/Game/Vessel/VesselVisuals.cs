using UnityEngine;

public class VesselVisuals : MonoBehaviour
{
    public SpriteRenderer hull;
    public SpriteRenderer shield;
    public SpriteRenderer engineTrail;
    public SpriteRenderer shuttle;

    public Transform laserEmit;
    public Transform torpedoEmit;

    public Transform weaponReceiveHull;
    public Transform weaponReceiveShield;

	public Transform boardingEmit;
	public Transform boardingReceive;

    public bool HullVisible { get => hull.enabled; set => hull.enabled = value; }
    public bool ShieldVisible { get => shield.enabled; set => shield.enabled = value; }
    public bool TrailVisible { get => engineTrail.enabled; set => engineTrail.enabled = value; }
    public bool ShuttleVisible { get => shuttle.enabled; set => shuttle.enabled = value; }

    public void ResetVisibility()
    {
        HullVisible = true;
        ShieldVisible = false;
        TrailVisible = false;
        ShuttleVisible = false;

        ResetTransforms();
    }
    public void ResetTransforms()
    {
        shuttle.flipX = false;
        shuttle.transform.position = boardingEmit.position;
    }
}
