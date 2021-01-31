using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
	private static Game instance;
	public static Game Instance
	{
		get
		{
			if (instance == null)
				instance = FindObjectOfType<Game>();
			return instance;
		}
	}

	public PageManager pageManager;
	public AudioManager audioManager;
    [System.NonSerialized] public Campaign campaign;
	[System.NonSerialized] public EffectCache effects;
	
	public void StartCampaign()
	{
		if (campaign != null)
			Destroy(campaign.gameObject);

		Campaign campaignPrefab = Resources.Load<Campaign>("Campaign");
		campaign = Instantiate(campaignPrefab);
		campaign.name = "Campaign";
		campaign.BeginCampaign("BalanceDefault");
	}

	private void Awake()
	{
		var effectsObj = new GameObject("Effects");
		effects = effectsObj.AddComponent<EffectCache>();
	}
}
