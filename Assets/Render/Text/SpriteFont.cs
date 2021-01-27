using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpriteFont", menuName = "Game/SpriteFont")]
public class SpriteFont : ScriptableObject
{
	public Sprite[] glyphs;
	
	[System.Serializable]
	public struct Translation
	{
		public char character;
		public Sprite glyph;
	}
	public Translation[] translations;

	private Dictionary<char, Sprite> characterToGlyph;
	private int maxWidth;

	private void OnValidate()
	{
		characterToGlyph = null;
	}
	
	private void Build()
	{
		if (characterToGlyph != null)
			return;

		characterToGlyph = new Dictionary<char, Sprite>();
		maxWidth = 0;

		foreach (var glyph in glyphs)
		{
			int width = Mathf.RoundToInt(glyph.rect.width);
			maxWidth = Mathf.Max(maxWidth, width);

			if (glyph.name.Length == 1)
				characterToGlyph.Add(glyph.name[0], glyph);
		}

		foreach (var translation in translations)
			characterToGlyph.Add(translation.character, translation.glyph);
	}

	public Sprite GetGlyph(char character)
	{
		Build();
		return characterToGlyph[character];
	}
	public int GetMaxWidth()
	{
		Build();
		return maxWidth;
	}
}
