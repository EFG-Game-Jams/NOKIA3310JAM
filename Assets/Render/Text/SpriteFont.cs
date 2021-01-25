using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpriteFont", menuName = "Game/SpriteFont", order = 1)]
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

	private void OnValidate()
	{
		characterToGlyph = null;
	}
	
	public Sprite GetGlyph(char character)
	{
		if (characterToGlyph == null)
		{
			characterToGlyph = new Dictionary<char, Sprite>();

			foreach (var glyph in glyphs)
				if (glyph.name.Length == 1)
					characterToGlyph.Add(glyph.name[0], glyph);

			foreach (var translation in translations)
				characterToGlyph.Add(translation.character, translation.glyph);
		}

		return characterToGlyph[character];
	}
}
