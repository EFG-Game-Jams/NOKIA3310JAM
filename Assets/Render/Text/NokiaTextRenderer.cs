using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NokiaTextRenderer : MonoBehaviour
{
    [SerializeField] private string initialText = "";
    public SpriteFont spriteFont;
    public int spacing = 1;

    public enum Align { Left, Center, Right };
    public Align align;

    private string text;
    public string Text
    {
        get => text;
        set => SetText(value);
    }

    private List<SpriteRenderer> glyphs = new List<SpriteRenderer>();

    void Start()
    {
        SetText(initialText);
    }

    private void RefreshGlyphs()
    {
        foreach (var glyph in glyphs)
            Destroy(glyph.gameObject);
        glyphs.Clear();

        for (int i = 0; i < text.Length; ++i)
        {
            GameObject glyphObject = new GameObject("Glyph");
            glyphObject.transform.parent = transform;

            SpriteRenderer glyph = glyphObject.AddComponent<SpriteRenderer>();
            glyph.sprite = spriteFont.GetGlyph(text[i]);

            glyphs.Add(glyph);
        }
    }
    public int GetWidth()
    {
        if (glyphs.Count == 0)
            return 0;

        int totalWidth = 0;
        foreach (var glyph in glyphs)
            totalWidth += Mathf.RoundToInt(glyph.bounds.size.x);

        int totalSpacing = spacing * (glyphs.Count - 1);
        return totalWidth + totalSpacing;
    }
    private void SnapPosition()
    {
        // snap position
        Vector3 pos = transform.position;
        pos.x = Mathf.Round(pos.x);
        pos.y = Mathf.Round(pos.y);
        transform.position = pos;
    }
    private void UpdateGlyphPositions()
    {
        SnapPosition();

        int offset = 0;
        switch (align)
        {
            case Align.Right:
                offset = -GetWidth();
                break;
            case Align.Center:
                offset = -GetWidth() / 2;
                break;
        }
                
        foreach (var glyph in glyphs)
        {
            glyph.transform.localPosition = new Vector3(offset, 0, 0);
            offset += Mathf.RoundToInt(glyph.bounds.size.x);
            offset += spacing;
        }
    }

    public void SetText(string text)
    {
        if (this.text == text)
            return;

        this.text = text;
        RefreshGlyphs();
        UpdateGlyphPositions();
    }
}
