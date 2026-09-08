using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Drawer for <see cref="InlineFieldAttribute"/>: renders a foldout whose label
/// carries values from the folded object, resolved by a property path template
/// like "X: {EndCell.X} Y: {EndCell.Y}".
/// </summary>
[CustomPropertyDrawer(typeof(InlineFieldAttribute))]
public class InlineFieldDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attr = ResolveAttribute(property);

        string value = GetInlineValue(property, attr);
        string finalLabel = string.IsNullOrEmpty(value) ? label.text : $"{label.text} ({value})";

        Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, finalLabel, true);

        if (!property.isExpanded)
            return;

        EditorGUI.indentLevel++;

        float y = position.y + EditorGUIUtility.singleLineHeight;

        var iterator = property.Copy();
        var end = iterator.GetEndProperty();
        bool enterChildren = true;

        while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, end))
        {
            float h = EditorGUI.GetPropertyHeight(iterator, true);
            EditorGUI.PropertyField(new Rect(position.x, y, position.width, h), iterator, true);
            y += h + 2;
            enterChildren = false;
        }

        EditorGUI.indentLevel--;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.isExpanded)
            return EditorGUIUtility.singleLineHeight;

        float height = EditorGUIUtility.singleLineHeight;

        var iterator = property.Copy();
        var end = iterator.GetEndProperty();
        bool enterChildren = true;

        while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, end))
        {
            height += EditorGUI.GetPropertyHeight(iterator, true) + 2;
            enterChildren = false;
        }

        return height;
    }

    private InlineFieldAttribute ResolveAttribute(SerializedProperty property)
    {
        var fieldAttrs = fieldInfo.GetCustomAttributes(typeof(InlineFieldAttribute), false);
        if (fieldAttrs.Length > 0)
            return (InlineFieldAttribute)fieldAttrs[0];

        var type = fieldInfo.FieldType;
        if (typeof(IList).IsAssignableFrom(type) && type.IsGenericType)
        {
            type = type.GetGenericArguments()[0];
        }

        var typeAttrs = type.GetCustomAttributes(typeof(InlineFieldAttribute), false);
        if (typeAttrs.Length > 0)
            return (InlineFieldAttribute)typeAttrs[0];

        return null;
    }

    private string GetInlineValue(SerializedProperty property, InlineFieldAttribute attr)
    {
        if (attr == null) return "";

        object target = GetTargetObject(property);
        if (target == null) return "";

        if (!string.IsNullOrEmpty(attr.Template))
            return BuildTemplate(target, attr.Template);

        if (target is IInlineInspectable inline)
            return inline.ToInspectorString();

        return "";
    }

    private string BuildTemplate(object target, string template)
    {
        return Regex.Replace(template, @"\{(.*?)\}", match =>
        {
            string path = match.Groups[1].Value;
            var val = GetValueByPath(target, path);
            return val?.ToString() ?? "null";
        });
    }

    private object GetValueByPath(object obj, string path)
    {
        var parts = path.Split('.');

        foreach (var partRaw in parts)
        {
            if (obj == null) return null;

            string part = partRaw;
            int? index = null;

            if (part.Contains("["))
            {
                int s = part.IndexOf("[");
                int e = part.IndexOf("]");
                index = int.Parse(part.Substring(s + 1, e - s - 1));
                part = part.Substring(0, s);
            }

            if (part == "Count")
            {
                if (obj is ICollection col)
                    return col.Count;

                if (obj is IEnumerable en)
                {
                    int c = 0;
                    var it = en.GetEnumerator();
                    while (it.MoveNext()) c++;
                    return c;
                }
            }

            var type = obj.GetType();

            var field = type.GetField(part);
            if (field != null)
                obj = field.GetValue(obj);
            else
            {
                var prop = type.GetProperty(part);
                if (prop != null)
                    obj = prop.GetValue(obj);
                else
                    return null;
            }

            if (index.HasValue)
            {
                var en = obj as IEnumerable;
                if (en == null) return null;

                var it = en.GetEnumerator();
                for (int i = 0; i <= index.Value; i++)
                {
                    if (!it.MoveNext()) return null;
                }
                obj = it.Current;
            }
        }

        return obj;
    }

    private object GetTargetObject(SerializedProperty property)
    {
        object obj = property.serializedObject.targetObject;
        string path = property.propertyPath.Replace(".Array.data[", "[");

        foreach (var element in path.Split('.'))
        {
            if (element.Contains("["))
            {
                var name = element.Substring(0, element.IndexOf("["));
                var index = int.Parse(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                obj = GetValue(obj, name, index);
            }
            else
            {
                obj = GetValue(obj, element);
            }
        }

        return obj;
    }

    private object GetValue(object source, string name)
    {
        if (source == null) return null;

        var type = source.GetType();

        var f = type.GetField(name);
        if (f != null) return f.GetValue(source);

        var p = type.GetProperty(name);
        if (p != null) return p.GetValue(source);

        return null;
    }

    private object GetValue(object source, string name, int index)
    {
        var enumerable = GetValue(source, name) as IEnumerable;
        if (enumerable == null) return null;

        var en = enumerable.GetEnumerator();
        for (int i = 0; i <= index; i++)
        {
            if (!en.MoveNext()) return null;
        }

        return en.Current;
    }
}

/// <summary>
/// Opt-in shorthand for <see cref="InlineFieldAttribute"/> without a template.
/// </summary>
public interface IInlineInspectable
{
    string ToInspectorString();
}
