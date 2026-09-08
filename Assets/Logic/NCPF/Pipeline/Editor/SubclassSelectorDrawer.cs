using System;
using System.Collections.Generic;
using NCPF.Pipeline.Presentation;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace NCPF.Pipeline.Editor
{
    /// <summary>
    /// Draws a field/item marked with <see cref="SubclassSelectorAttribute"/> as a concrete-type
    /// picker over the declared field's heirs. On a List&lt;T&gt; the attribute applies per element,
    /// so the container is left to Unity's default drawer and each element is drawn as one managed
    /// reference. A single field/item is a section: a dark header bar with the slot name, a type
    /// picker (<see cref="AdvancedDropdown"/>), then the instance fields. Heirs are gathered via
    /// <see cref="TypeCache"/> across all assemblies.
    /// </summary>
    [CustomPropertyDrawer(typeof(SubclassSelectorAttribute))]
    public class SubclassSelectorDrawer : PropertyDrawer
    {
        private const float Pad = 2f;
        private const float HeaderLeftPad = 6f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.isArray)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            DrawSingle(position, property, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isArray) return EditorGUI.GetPropertyHeight(property, label, true);
            return GetSingleHeight(property);
        }

        private Type FieldType
        {
            get
            {
                Type t = fieldInfo.FieldType;
                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>))
                {
                    t = t.GetGenericArguments()[0];
                }
                return t;
            }
        }

        /// <summary>Concrete (non-abstract, parameterless ctor, non-obsolete) heirs of the field type.</summary>
        private List<Type> GetConcreteTypes()
        {
            Type baseType = FieldType;
            List<Type> result = new List<Type>();
            foreach (Type derived in TypeCache.GetTypesDerivedFrom(baseType))
            {
                if (derived.IsAbstract || derived.IsGenericTypeDefinition) continue;
                if (derived.GetConstructor(Type.EmptyTypes) == null) continue;
                if (Attribute.IsDefined(derived, typeof(ObsoleteAttribute))) continue;
                result.Add(derived);
            }
            return result;
        }

        private static string DisplayName(Type t)
        {
            return t == null ? "None" : ObjectNames.NicifyVariableName(t.Name);
        }

        private class TypeDropdownItem : AdvancedDropdownItem
        {
            public readonly Type Type;

            public TypeDropdownItem(Type t) : base(DisplayName(t))
            {
                Type = t;
            }
        }

        private class TypePicker : AdvancedDropdown
        {
            private readonly List<Type> _types;
            private readonly Action<Type> _onSelected;

            public TypePicker(AdvancedDropdownState state, List<Type> types, Action<Type> onSelected)
                : base(state)
            {
                _types = types;
                _onSelected = onSelected;
                minimumSize = new Vector2(230f, 320f);
            }

            protected override AdvancedDropdownItem BuildRoot()
            {
                AdvancedDropdownItem root = new AdvancedDropdownItem("Select Type");
                root.AddChild(new TypeDropdownItem(null));
                foreach (Type t in _types)
                {
                    root.AddChild(new TypeDropdownItem(t));
                }
                return root;
            }

            protected override void ItemSelected(AdvancedDropdownItem item)
            {
                _onSelected?.Invoke((item as TypeDropdownItem)?.Type);
            }
        }

        private void DrawSingle(Rect position, SerializedProperty property, GUIContent label)
        {
            List<Type> types = GetConcreteTypes();
            Type current = property.managedReferenceValue?.GetType();

            Rect headerRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            DrawHeader(headerRect, HeaderLabel(label, property));

            float y = headerRect.yMax + Pad;

            Rect pickerRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.BeginProperty(pickerRect, GUIContent.none, property);
            if (EditorGUI.DropdownButton(pickerRect, new GUIContent(DisplayName(current)), FocusType.Keyboard, EditorStyles.popup))
            {
                TypePicker picker = new TypePicker(new AdvancedDropdownState(), types, t => Assign(property, t));
                picker.Show(pickerRect);
            }
            EditorGUI.EndProperty();
            y += EditorGUIUtility.singleLineHeight + Pad;

            if (property.managedReferenceValue != null)
            {
                EditorGUI.indentLevel++;
                Rect childRect = new Rect(position.x, y, position.width, GetChildrenHeight(property));
                DrawChildren(childRect, property);
                EditorGUI.indentLevel--;
            }
        }

        private float GetSingleHeight(SerializedProperty property)
        {
            float h = EditorGUIUtility.singleLineHeight + Pad + EditorGUIUtility.singleLineHeight;
            if (property.managedReferenceValue != null)
            {
                h += Pad + GetChildrenHeight(property);
            }
            return h;
        }

        /// <summary>Slot name for a single field, "Override N" for a list element.</summary>
        private static string HeaderLabel(GUIContent label, SerializedProperty property)
        {
            string path = property.propertyPath;
            int marker = path.IndexOf(".Array.data[");
            if (marker >= 0)
            {
                int start = marker + ".Array.data[".Length;
                int end = path.IndexOf(']', start);
                if (end > start && int.TryParse(path.Substring(start, end - start), out int i))
                {
                    return "Override " + i;
                }
            }
            return label.text;
        }

        private static void DrawHeader(Rect rect, string text)
        {
            EditorGUI.DrawRect(rect, HeaderBg);
            Color prevColor = GUI.color;
            GUI.color = HeaderText;
            Rect labelRect = new Rect(rect.x + HeaderLeftPad, rect.y, rect.width - HeaderLeftPad, rect.height);
            EditorGUI.LabelField(labelRect, text, EditorStyles.boldLabel);
            GUI.color = prevColor;
        }

        private static Color HeaderBg => EditorGUIUtility.isProSkin
            ? new Color(0.18f, 0.18f, 0.18f, 1f)
            : new Color(0.55f, 0.55f, 0.55f, 1f);

        private static Color HeaderText => EditorGUIUtility.isProSkin
            ? new Color(0.85f, 0.85f, 0.85f, 1f)
            : new Color(0.97f, 0.97f, 0.97f, 1f);

        private static void Assign(SerializedProperty property, Type type)
        {
            property.managedReferenceValue = type == null ? null : Activator.CreateInstance(type);
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }

        private static void DrawChildren(Rect position, SerializedProperty property)
        {
            float y = position.y;
            SerializedProperty iter = property.Copy();
            SerializedProperty end = iter.GetEndProperty();
            bool enterChildren = true;
            while (iter.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iter, end))
            {
                float h = EditorGUI.GetPropertyHeight(iter, true);
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, h), iter, true);
                y += h + Pad;
                enterChildren = false;
            }
        }

        private static float GetChildrenHeight(SerializedProperty property)
        {
            float h = 0f;
            SerializedProperty iter = property.Copy();
            SerializedProperty end = iter.GetEndProperty();
            bool enterChildren = true;
            while (iter.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iter, end))
            {
                h += EditorGUI.GetPropertyHeight(iter, true) + Pad;
                enterChildren = false;
            }
            return h;
        }
    }
}
