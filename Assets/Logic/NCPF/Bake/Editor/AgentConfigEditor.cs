using Core.Foundation;
using NCPF.Domain;
using NCPF.Shared.Presentation;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Inspector preview of any <see cref="AgentConfig"/> footprint: the discrete mask at a chosen cell
/// size and heading, with the anchor cell highlighted — the cell the trace is applied to the agent
/// position with, the one that must match the point the body is dragged by in game. Cell size and
/// heading are preview-only and never stored in the asset.
/// </summary>
[CustomEditor(typeof(AgentConfig), true)]
public class AgentConfigEditor : Editor
{
    private Texture2D _previewTexture;
    private float _cellSize = 0.1f;
    private float _angle;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);
        _cellSize = Mathf.Clamp(EditorGUILayout.FloatField("Preview Cell Size", _cellSize), 0.01f, 999f);
        _angle = EditorGUILayout.Slider("Preview Angle", _angle, 0f, 360f);

        Agent agent = ((AgentConfig)target).GetAgent(_cellSize);
        if (agent == null)
            return;

        agent.Angle = _angle;
        DrawPreview(agent);
    }

    private void OnDisable()
    {
        if (_previewTexture != null)
        {
            DestroyImmediate(_previewTexture);
            _previewTexture = null;
        }
    }

    private void DrawPreview(Agent agent)
    {
        bool[,] footprint = agent.Footprint;
        int size = footprint.GetLength(0);
        if (_previewTexture == null ||
            _previewTexture.width != size ||
            _previewTexture.height != size)
        {
            if (_previewTexture != null)
            {
                DestroyImmediate(_previewTexture);
            }
            _previewTexture = new Texture2D(size, size);
            _previewTexture.filterMode = FilterMode.Point;
            _previewTexture.hideFlags = HideFlags.HideAndDontSave;
        }

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                _previewTexture.SetPixel(x, y, footprint[x, y] ? Color.white : Color.black);
            }
        }

        Int2 anchor = agent.Anchor;
        if (anchor.x >= 0 && anchor.y >= 0 && anchor.x < size && anchor.y < size)
        {
            _previewTexture.SetPixel(anchor.x, anchor.y, Color.blue);
        }

        _previewTexture.Apply();
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
