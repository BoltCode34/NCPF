using NCPF.Shared.Presentation;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Inspector preview of a baked <see cref="MapAsset"/>: per-heading occupancy
/// layers rendered as a texture. Works through reflection over the concrete
/// asset's fields, so no assembly references NCPF.Config.
/// </summary>
[CustomEditor(typeof(MapAsset), true)]
public class NCPFMapEditor : Editor
{
    private int _viewLayerIndex = 0;
    private Texture2D _previewTexture;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Type assetType = target.GetType();
        FieldInfo mapField = assetType
            .GetField("_map", BindingFlags.NonPublic | BindingFlags.Instance);

        if (mapField == null)
        {
            EditorGUILayout.HelpBox("Field _map not found.", MessageType.Error);
            return;
        }

        Array layers = (Array)mapField.GetValue(target);

        if (layers == null || layers.Length == 0)
        {
            EditorGUILayout.HelpBox("Map is empty.", MessageType.Info);
            return;
        }

        Type layerType = layers.GetType().GetElementType();
        FieldInfo layerDataField = layerType.GetField("Map");
        FieldInfo layerAngleField = layerType.GetField("Angle");

        bool[] firstLayerData = (bool[])layerDataField.GetValue(layers.GetValue(0));

        if (firstLayerData == null || firstLayerData.Length == 0)
        {
            EditorGUILayout.HelpBox("Layer data is empty.", MessageType.Warning);
            return;
        }

        int layerCount = layers.Length;
        float angleStep = (360f / layerCount);

        EditorGUILayout.LabelField("Layers", layerCount.ToString());
        EditorGUILayout.LabelField("Angle Step", angleStep + "°");

        FieldInfo sizeXField = assetType.GetField("SizeX", BindingFlags.Public | BindingFlags.Instance);
        FieldInfo sizeYField = assetType.GetField("SizeY", BindingFlags.Public | BindingFlags.Instance);
        int sizeX = sizeXField != null ? (int)sizeXField.GetValue(target) : 0;
        int sizeY = sizeYField != null ? (int)sizeYField.GetValue(target) : 0;

        EditorGUILayout.LabelField("Layer Size", $"{sizeX} x {sizeY}");

        _viewLayerIndex = EditorGUILayout.IntSlider(
            "View Layer",
            _viewLayerIndex,
            0,
            layerCount - 1
        );

        DrawLayerPreview(layers.GetValue(_viewLayerIndex), layerDataField, layerAngleField, sizeX, sizeY);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawLayerPreview(object layer, FieldInfo layerDataField, FieldInfo layerAngleField, int sizeX, int sizeY)
    {
        bool[] layerData = (bool[])layerDataField.GetValue(layer);
        if (layerData == null || layerData.Length == 0)
            return;

        if (_previewTexture == null ||
            _previewTexture.width != sizeX ||
            _previewTexture.height != sizeY)
        {
            _previewTexture = new Texture2D(sizeX, sizeY);
            _previewTexture.filterMode = FilterMode.Point;
        }

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                bool occupied = layerData[x * sizeY + y];
                _previewTexture.SetPixel(
                    x,
                    y,
                    occupied ? Color.white : Color.black
                );
            }
        }

        _previewTexture.Apply();
        GUILayout.Space(10);
        GUILayout.Label($"Angle: {layerAngleField?.GetValue(layer)}°");
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
