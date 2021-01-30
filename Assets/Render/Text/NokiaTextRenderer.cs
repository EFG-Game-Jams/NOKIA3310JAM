using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter))]
public class NokiaTextRenderer : MonoBehaviour
{
    public enum Align { Left, Center, Right };

    public string initialText = "";
    public SpriteFont spriteFont;
    public int spacing = 1;
    public bool monospace = false;
    public Align align;

    private struct Glyph
    {
        public Sprite sprite;
        public int spriteWidth;
        public int spriteOffset;
        public int glyphWidth;
        public int glyphOffset;
    }

    private string text;
    private List<Glyph> glyphs = new List<Glyph>();
    private Mesh mesh;

    public string Text
    {
        get => text;
        set => SetText(value);
    }

    private void RefreshGlyphs()
    {
        // fetch glyphs
        int monoWidth = spriteFont.GetMaxWidth();
        int monoOffset = monoWidth / 2;
        int offset = 0;

        glyphs.Clear();
        for (int i = 0; i < text.Length; ++i)
        {
            Glyph glyph = new Glyph();
            glyph.sprite = spriteFont.GetGlyph(text[i]);
            glyph.spriteWidth = Mathf.RoundToInt(glyph.sprite.rect.width);
            glyph.spriteOffset = (monospace ? monoOffset - glyph.spriteWidth / 2 : 0);
            glyph.glyphWidth = (monospace ? monoWidth : glyph.spriteWidth);
            glyph.glyphOffset = offset;
            glyphs.Add(glyph);

            offset += glyph.glyphWidth + spacing;
        }

        // apply alignment offset
        int totalWidth = offset - spacing;
        int alignOffset = 0;
        switch (align)
        {
            case Align.Right:
                alignOffset = -totalWidth;
                break;
            case Align.Center:
                alignOffset = -totalWidth / 2;
                break;
        }
        for (int i = 0; i < glyphs.Count; ++i)
        {
            Glyph glyph = glyphs[i];
            glyph.glyphOffset += alignOffset;
            glyphs[i] = glyph;
        }
    }
    private void RebuildMesh(int maxGlyphs = int.MaxValue)
    {
        maxGlyphs = Mathf.Min(maxGlyphs, glyphs.Count);

        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        int startIndex = 0;
        for (int i = 0; i < maxGlyphs; ++i)
        {
            Glyph glyph = glyphs[i];

            Vector3 offset = Vector2.right * glyph.spriteOffset - glyph.sprite.pivot;
            Vector3 origin = Vector3.right * glyph.glyphOffset + offset;
            Vector3 right = Vector3.right * glyph.spriteWidth;
            Vector3 up = Vector3.up * glyph.sprite.rect.height;

            vertices.Add(origin);
            vertices.Add(origin + up);
            vertices.Add(origin + right + up);
            vertices.Add(origin + right);

            Rect uvRect = glyph.sprite.rect;
            Vector2 scale = new Vector2(1f / glyph.sprite.texture.width, 1f / glyph.sprite.texture.height);
            uvs.Add(new Vector2(uvRect.xMin, uvRect.yMin) * scale);
            uvs.Add(new Vector2(uvRect.xMin, uvRect.yMax) * scale);
            uvs.Add(new Vector2(uvRect.xMax, uvRect.yMax) * scale);
            uvs.Add(new Vector2(uvRect.xMax, uvRect.yMin) * scale);

            indices.Add(startIndex);
            indices.Add(startIndex + 1);
            indices.Add(startIndex + 2);
            indices.Add(startIndex);
            indices.Add(startIndex + 2);
            indices.Add(startIndex + 3);
            startIndex += 4;
        }

        if (mesh == null)
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().sharedMesh = mesh;
        }

        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uvs);
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);
    }
    private void SnapPosition()
    {
        // snap position
        Vector3 pos = transform.localPosition;
        pos.x = Mathf.Round(pos.x);
        pos.y = Mathf.Round(pos.y);
        transform.localPosition = pos;
    }

    public void SetText(string text)
    {
        if (this.text == text)
            return;

        this.text = text;
        RefreshGlyphs();
        RebuildMesh();
    }

    private IEnumerator Animate(float interval)
    {
        float time = 0;
        for (int i = 0; i < glyphs.Count; ++i)
        {
            RebuildMesh(i + 1);
            while (time < (i + 1) * interval)
            {
                yield return null;
                time += Time.deltaTime;
            }
        }
    }
    public IEnumerator AnimateInterval(string text, float interval)
    {
        text = text ?? initialText;

        SetText(text);
        return Animate(interval);
    }
    public IEnumerator AnimateDuration(string text, float duration)
    {
        text = text ?? initialText;
        return AnimateInterval(text, duration / text.Length);
    }

    private void Awake()
    {
        GetComponent<MeshRenderer>().sharedMaterial = spriteFont.material;
        SetText(initialText);
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Application.isPlaying)
            return; // don't execute in play mode

        SnapPosition();
        if (mesh == null || UnityEditor.EditorUtility.IsDirty(this))
        {
            SetText("");
            SetText(initialText);
        }
    }
#endif
}
