using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VesselStats))]
public class VesselSystemStatus : MonoBehaviour
{
	private VesselStats stats;

	public int health;
	public int engines;
	public int weapons;
	public int shields;

	private void Awake()
	{
		stats = GetComponent<VesselStats>();
	}


}
