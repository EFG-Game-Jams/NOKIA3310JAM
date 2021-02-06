using UnityEngine;

[CreateAssetMenu(fileName = "NewEncounterDescriptor", menuName = "Game/Encounter")]
public class EncounterDescriptor : ScriptableObject
{
    public enum Category
    {
        Hostile,
        Friendly,
        Special,
    }

    public Category category;
    public VesselVisuals visuals;

    [Tooltip("Base stats of the opponent, scaling is added onto this")]
    public VesselStatValues baseStats;

    [Tooltip("Total scaling stat points are distributed according to these weights")]
    public VesselStatWeights statsScalingWeight;

    [Tooltip("Total scaling stat points = statsScaling * totalPlayerStatPoints")]
    public float statsScaling = 1f;

	public VesselModifiers playerModifiers;
	public VesselModifiers enemyModifiers;

    public int initialAmmo = 1;

	public AiBehaviour enemyAiBehaviour;
}
