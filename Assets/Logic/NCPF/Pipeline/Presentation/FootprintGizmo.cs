using Core.Foundation;
using NCPF.Domain;
using NCPF.Shared.Presentation;
using System.Collections.Generic;
using UnityEngine;

namespace NCPF.Pipeline.Presentation
{
    /// <summary>
    /// Draws an <see cref="AgentConfig"/> footprint as a translucent texture with its anchor on the
    /// given position. The footprint is rasterised at a fine cell size, independent of any map, and
    /// cached per heading step; the cache drops itself when the drawn shape changes. Owned by the
    /// component that draws, released with it.
    /// </summary>
    public class FootprintGizmo
    {
        private const int Resolution = 64;
        private const float AngleStep = 5f;
        private static readonly Color Fill = new Color(0f, 0.45f, 1f, 0.45f);
        private static Material _material;

        private readonly Dictionary<int, Entry> _entries = new Dictionary<int, Entry>();
        private AgentConfig _shape;
        private Vector2 _size;
        private Vector2 _anchor;

        public void Draw(AgentConfig shape, Vector2 position, float angle)
        {
            if (shape == null)
            {
                return;
            }

            if (shape != _shape || shape.FootprintSize != _size || shape.FootprintAnchor != _anchor)
            {
                Release();
                _shape = shape;
                _size = shape.FootprintSize;
                _anchor = shape.FootprintAnchor;
            }

            float cellSize = Mathf.Max(_size.x, _size.y) / Resolution;
            if (cellSize <= 0f)
            {
                return;
            }

            int buckets = Mathf.RoundToInt(360f / AngleStep);
            int bucket = Mathf.RoundToInt(Mathf.Repeat(angle, 360f) / AngleStep) % buckets;

            if (!_entries.TryGetValue(bucket, out Entry entry) || entry.Texture == null)
            {
                Agent agent = shape.GetAgent(cellSize);
                agent.Angle = bucket * AngleStep;
                entry = new Entry(FootprintTexture.Build(agent.Footprint, Fill, Color.clear), agent.Anchor);
                _entries[bucket] = entry;
            }

            Material material = QuadMaterial();
            if (material == null)
            {
                return;
            }

            float left = position.x - (entry.Anchor.x + 0.5f) * cellSize;
            float bottom = position.y - (entry.Anchor.y + 0.5f) * cellSize;
            float right = left + entry.Texture.width * cellSize;
            float top = bottom + entry.Texture.height * cellSize;

            material.mainTexture = entry.Texture;
            material.SetPass(0);
            GL.PushMatrix();
            GL.Begin(GL.QUADS);
            GL.Color(Color.white);
            GL.TexCoord2(0f, 0f); GL.Vertex3(left, bottom, 0f);
            GL.TexCoord2(0f, 1f); GL.Vertex3(left, top, 0f);
            GL.TexCoord2(1f, 1f); GL.Vertex3(right, top, 0f);
            GL.TexCoord2(1f, 0f); GL.Vertex3(right, bottom, 0f);
            GL.End();
            GL.PopMatrix();
        }

        private static Material QuadMaterial()
        {
            if (_material == null)
            {
                Shader shader = Shader.Find("Sprites/Default");
                if (shader == null)
                {
                    return null;
                }
                _material = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
            }
            return _material;
        }

        public void Release()
        {
            foreach (Entry entry in _entries.Values)
            {
                FootprintTexture.Release(entry.Texture);
            }
            _entries.Clear();
            _shape = null;
        }

        private readonly struct Entry
        {
            public readonly Texture2D Texture;
            public readonly Int2 Anchor;

            public Entry(Texture2D texture, Int2 anchor)
            {
                Texture = texture;
                Anchor = anchor;
            }
        }
    }
}
