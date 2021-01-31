using UnityEngine;

[System.Serializable]
public class VesselModifiers
{
    [Header("Subsystems")]
    [SerializeField]
    public bool HasShields = true;

    [SerializeField]
    public bool HasEngines = true;

    [SerializeField]
    public bool HasWeapons = true;

    [Header("Abilities")]
    [SerializeField]
    public bool CanMissle = true;

    [SerializeField]
    public bool CanBoard = true;

    [SerializeField]
    public bool CanRepel = true;

    [SerializeField]
    public bool CanScan = true;

    [Header("Constraints")]
    [SerializeField]
    public bool Boardable = true;
}