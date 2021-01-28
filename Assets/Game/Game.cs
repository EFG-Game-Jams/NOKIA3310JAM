﻿using System.Collections;
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
	public Campaign campaign;

	public void StartCampaign()
	{
		if (campaign != null)
			Destroy(campaign.gameObject);

		Campaign campaignPrefab = Resources.Load<Campaign>("Campaign");
		campaign = Instantiate(campaignPrefab);
		campaign.Begin("BalanceDefault");
	}
}