using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VesselStats : MonoBehaviour
{
	const int MaxStatValue = 10;

	public enum Type
	{
		Durability,
		Attack,
		Defense,
		Luck
	}

	[SerializeField] private int[] statValues;
	private StatRoll[] statRolls;

	private void Awake()
	{
		statValues = new int[4];

		statRolls = new StatRoll[4];
		statRolls[0] = null;
		statRolls[1] = Resources.Load<StatRoll>("StatRollAttack");
		statRolls[2] = Resources.Load<StatRoll>("StatRollDefense");
		statRolls[3] = Resources.Load<StatRoll>("StatRollLuck");
	}

	public int GetRaw(Type type)
	{
		return statValues[(int)type];
	}
	public void SetRaw(Type type, int value)
	{
		if (value < 0 || value > MaxStatValue)
			throw new System.Exception("Attempt to set stat " + type.ToString() + " to out-of-range value " + value);
		statValues[(int)type] = value;
	}

	private float Roll(Type type, float scale = 1f)
	{
		StatRoll roller = statRolls[(int)type];
		Debug.Assert(roller != null);
		return roller.Roll(GetRaw(type), MaxStatValue, scale);
	}

	public float GetDurability() => GetRaw(Type.Durability) / (float)MaxStatValue;
	public float RollAttack(float scale = 1f) => Roll(Type.Attack, scale);
	public float RollDefense(float scale = 1f) => Roll(Type.Defense, scale);
	public float RollLuck(float scale = 1f) => Roll(Type.Luck, scale);

	public int GetMaxTransferable(Type from, Type to)
	{
		int maxSend = GetRaw(from);
		int maxReceive = MaxStatValue - GetRaw(to);
		return Mathf.Min(maxSend, maxReceive);
	}
	public int Transfer(Type from, Type to, int maxAmount)
	{
		int amount = Mathf.Min(maxAmount, GetMaxTransferable(from, to));
		statValues[(int)from] -= amount;
		statValues[(int)to] += amount;
		return amount;
	}
}
