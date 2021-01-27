using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStatRoll", menuName = "Game/StatRoll")]
public class StatRoll : ScriptableObject
{
	[Range(0, 1)]
	[Tooltip("Where the distribution starts relative to the stat's value")]
	public float offset = 0f;

	[Range(0, 1)]
	[Tooltip("How much the stat's value affects the distribution")]
	public float biasStrength = 1f;

	[Range(0, 1)]
	[Tooltip("Shape of the stat-induced bias applied to the dice roll\n0.5 is a linear bias")]
	public float biasShape = .5f;

	public float Roll(int statValue, int statMax = 10, float scale = 1f)
	{
		float stat = statValue / (float)statMax;
		float scaledOffset = stat * offset;
		float roll = scaledOffset + Distribution.CurvedBias(1f - biasShape, stat * biasStrength) * (1f - scaledOffset);
		roll = Mathf.Clamp(roll, 0f, 1f);
		return roll * scale;
	}
}
