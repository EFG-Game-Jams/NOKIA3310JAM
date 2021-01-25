using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NokiaTextRenderer : MonoBehaviour
{
    [SerializeField] private string initialText = "";
    public SpriteFont spriteFont;
    public int spacing = 1;
    public bool monospace = false;

    public enum Align { Left, Center, Right };
    public Align align;

    [Tooltip("When enabled, clicking on a glyph will redirect selection to this object")]
    public bool interceptGlyphSelection = true;

    private string text;
    public string Text
    {
        get => text;
        set => SetText(value);
    }

    private List<SpriteRenderer> glyphs = new List<SpriteRenderer>();

    private void RefreshGlyphs()
    {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(transform.childCount - 1).gameObject);
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
        if (monospace)
        {
            totalWidth = glyphs.Count * spriteFont.GetMaxWidth();
        }
        else
        {
            foreach (var glyph in glyphs)
                totalWidth += Mathf.RoundToInt(glyph.bounds.size.x);
        }

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

        if (monospace)
        {
            int monoWidth = spriteFont.GetMaxWidth();
            int monoOffset = monoWidth / 2;
            foreach (var glyph in glyphs)
            {
                int width = Mathf.RoundToInt(glyph.bounds.size.x);
                glyph.transform.localPosition = new Vector3(offset + monoOffset - (width / 2), 0, 0);
                offset += monoWidth + spacing;
            }
        }
        else
        {
            foreach (var glyph in glyphs)
            {
                glyph.transform.localPosition = new Vector3(offset, 0, 0);
                offset += Mathf.RoundToInt(glyph.bounds.size.x) + spacing;
            }
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
    
    private void Update()
    {
#if UNITY_EDITOR
        if (UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null)
        {
            // we're in prefab mode, clear glyphs and stop
            transform.localPosition = Vector3.zero;
            SetText("");
            return;
        }
        
        if (interceptGlyphSelection)
        {
            Transform selected = UnityEditor.Selection.activeTransform;
            if (selected != null && selected.parent == transform)
            {
                // naughty stuff to collapse this object in the hierarchy window
                UnityEditor.EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
                var hierarchyWindow = UnityEditor.EditorWindow.focusedWindow;
                var expandMethodInfo = hierarchyWindow.GetType().GetMethod("SetExpandedRecursive");
                expandMethodInfo.Invoke(hierarchyWindow, new object[] { gameObject.GetInstanceID(), false });

                // redirect selection
                UnityEditor.Selection.activeTransform = transform;                
            }
        }   
#endif

        SnapPosition();
        SetText("");
        SetText(initialText);
    }
}
