﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageManager : MonoBehaviour
{
	#region Data

	public string defaultPage;

	private List<Page> pageStack = new List<Page>();
	private Page[] pages;

	#endregion

	#region Page management

	// get a page by name
	public Page GetPageByName(string name)
	{
		foreach (var page in pages)
			if (page.gameObject.name == name)
				return page;
		throw new System.Exception("Page '" + name + "' not found");
	}

	// get the currently active page, or null
	public Page GetActivePage()
	{
		if (pageStack.Count > 0)
			return pageStack[pageStack.Count - 1];
		return null;
	}

	// push a page to the stack
	public void PushPage(Page page)
	{
		pageStack.Add(page);
		ActivateTop();
	}
	public void PushPage(string name)
	{
		PushPage(GetPageByName(name));
	}

	// pop a page from the stack
	public void PopPage()
	{
		pageStack.RemoveAt(pageStack.Count - 1);
		ActivateTop();
	}

	// clear the stack and push a page
	public void SetPage(Page page)
	{
		pageStack.Clear();
		PushPage(page);
	}
	public void SetPage(string name)
	{
		SetPage(GetPageByName(name));
	}

	#endregion

	#region Private API

	private void SetActive(Page page, bool active)
	{
		page.gameObject.SetActive(active);

		bool wasActive = page.isActive;
		page.isActive = active;

		if (active && !wasActive)
			page.OnActivate();
		else if (!active && wasActive)
			page.OnDeactivate();
	}

	private void ActivateTop()
	{
		// activate top
		Page top = pageStack[pageStack.Count - 1];

		// deactivate all
		foreach (var page in pages)
			SetActive(page, page == top);
	}

	#endregion

	#region Unity callbacks

	private void Awake()
	{
		// load and instantiate all pages
		var pagePrefabs = Resources.LoadAll<Page>("Pages");
		pages = new Page[pagePrefabs.Length];
		for (int i = 0; i < pages.Length; ++i)
		{
			pages[i] = Instantiate(pagePrefabs[i], transform);
			pages[i].pageManager = this;
			pages[i].gameObject.name = pagePrefabs[i].gameObject.name;
		}
	}

	private void Start()
	{
		// set default page
		SetPage(defaultPage);
	}

	private void Update()
	{
		if (GameInput.TryGetAction(out GameInput.Action inputAction))
			GetActivePage()?.OnInput(inputAction);
	}

	#endregion
}
