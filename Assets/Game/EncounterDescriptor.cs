using System.Collections;
using System.Collections.Generic;
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
	public int[] statsBase = new int[4];
	[Tooltip("Total scaling stat points are distributed according to these weights")]
	public float[] statsScalingWeight = new float[] { 1, 1, 1, 1 };
	[Tooltip("Total scaling stat points = statsScaling * totalPlayerStatPoints")]
	public float statsScaling = 1f;
}
