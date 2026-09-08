using Core.Foundation;
using NCPF.Bake.Presentation;
using NCPF.Domain;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Inspector preview of the rectangular footprint: the discrete mask with the
/// anchor cell highlighted — the cell the trace is applied to the agent
/// position with, the one that must match the point the body is dragged by in
/// game.
/// </summary>
[CustomEditor(typeof(RectangularConfig))]
public class RectangularConfigEditor : Editor
{
    private Texture2D _previewTexture;
    private float _cellSize;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        RectangularConfig config = (RectangularConfig)target;
        _cellSize = Mathf.Clamp(EditorGUILayout.FloatField("Cell size", _cellSize), 0.01f, 999f);
        Agent agent = config.GetAgent(_cellSize);
        DrawPreview(agent);
    }

    private void DrawPreview(Agent agent)
    {
        if (agent == null)
            return;

        bool[,] footprint = agent.Footprint;
        int size = footprint.GetLength(0);
        if (_previewTexture == null ||
            _previewTexture.width != size ||
            _previewTexture.height != size)
        {
            _previewTexture = new Texture2D(size, size);
            _previewTexture.filterMode = FilterMode.Point;
        }

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                bool occupied = footprint[x, y];
                _previewTexture.SetPixel(
                    x,
                    y,
                    occupied ? Color.white : Color.black
                );
            }
        }

        Int2 anchor = agent.Anchor;
        if (anchor.x >= 0 && anchor.y >= 0 && anchor.x < size && anchor.y < size)
        {
            _previewTexture.SetPixel(anchor.x, anchor.y, Color.blue);
        }

        _previewTexture.Apply();
        GUILayout.Space(10);
        GUILayout.Label($"Angle: {agent.Angle}°");
        GUILayout.Label($"Anchor cell: ({anchor.x}, {anchor.y}) of {size}×{size}");
        float width = Mathf.Clamp(
            EditorGUIUtility.currentViewWidth - 40f,
            256f,
            600f
        );

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        Rect rect = GUILayoutUtility.GetRect(width, width);

        EditorGUI.DrawPreviewTexture(
            rect,
            _previewTexture,
            null,
            ScaleMode.ScaleToFit
        );

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
}
