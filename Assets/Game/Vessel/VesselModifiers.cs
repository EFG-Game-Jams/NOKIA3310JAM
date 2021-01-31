using UnityEngine;

[System.Serializable]
public class VesselModifiers
{
	[Header("Subsystems")]
    [SerializeField]
    public bool HasShields = true;

    [SerializeField]
    public bool HasWeapons = true;

    [SerializeField]
    public bool HasEngines = true;

    [SerializeField]
    public bool HasLasers = true;

	[Header("Abilities")]
    [SerializeField]
    public bool HasMissles = true;

    [SerializeField]
    public bool HasScan = true;
}