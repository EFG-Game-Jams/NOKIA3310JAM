using UnityEngine;

[System.Serializable]
public class VesselModifiers
{
    [Header("Abilities")]
    [SerializeField]
    public bool CanRaiseShields = true;

    [SerializeField]
    public bool CanLaser = true;

    [SerializeField]
    public bool CanMissle = true;

    [SerializeField]
    public bool CanBoard = true;

    [SerializeField]
    public bool CanRepel = true;

    [SerializeField]
    public bool CanRepair = true;

    [SerializeField]
    public bool CanScan = true;

    [SerializeField]
    public bool CanFlee = true;

    [SerializeField]
    public bool CanEvade = true;
}