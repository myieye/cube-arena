using System;
using CubeArena.Assets.MyScripts.Utils.Attributes;
using UnityEditor;
using UnityEngine;

namespace CubeArena.Assets.Editor.Utils {

    [CustomPropertyDrawer (typeof (ConditionalFieldAttribute))]
    public class ConditionalFieldAttributeDrawer : PropertyDrawer {

        private ConditionalFieldAttribute Attribute {
            get {
                return _attribute ?? (_attribute = attribute as ConditionalFieldAttribute);
            }
        }
        private string PropertyToCheck {
            get {
                return Attribute != null ? _attribute.PropertyToCheck : null;
            }
        }
        private object CompareValue {
            get {
                return Attribute != null ? _attribute.CompareValue : null;
            }
        }

        private ConditionalFieldAttribute _attribute;
        private bool _toShow = true;

        public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
            return _toShow ? EditorGUI.GetPropertyHeight (property) : 0;
        }

        // TODO: Skip array fields
        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
            if (!string.IsNullOrEmpty (PropertyToCheck)) {
                var conditionProperty = FindPropertyRelative (property, PropertyToCheck);
                if (conditionProperty != null) {
                    bool isBoolMatch = conditionProperty.propertyType == SerializedPropertyType.Boolean && conditionProperty.boolValue;
                    string compareStringValue = CompareValue != null ? CompareValue.ToString ().ToUpper () : "NULL";
                    if (isBoolMatch && compareStringValue == "FALSE") isBoolMatch = false;

                    string conditionPropertyStringValue = conditionProperty.AsStringValue ().ToUpper ();
                    bool objectMatch = compareStringValue == conditionPropertyStringValue;

                    if (!isBoolMatch && !objectMatch) {
                        _toShow = false;
                        return;
                    }
                }
            }

            _toShow = true;
            EditorGUI.PropertyField (position, property, label, true);
        }

        private SerializedProperty FindPropertyRelative (SerializedProperty property, string toGet) {
            if (property.depth == 0) return property.serializedObject.FindProperty (toGet);

            var path = property.propertyPath.Replace (".Array.data[", "[");
            var elements = path.Split ('.');
            SerializedProperty parent = null;

            for (int i = 0; i < elements.Length - 1; i++) {
                var element = elements[i];
                int index = -1;
                if (element.Contains ("[")) {
                    index = Convert.ToInt32 (element.Substring (element.IndexOf ("[", StringComparison.Ordinal)).Replace ("[", "").Replace ("]", ""));
                    element = element.Substring (0, element.IndexOf ("[", StringComparison.Ordinal));
                }

                parent = i == 0 ?
                    property.serializedObject.FindProperty (element) :
                    parent.FindPropertyRelative (element);

                if (index >= 0) parent = parent.GetArrayElementAtIndex (index);
            }

            return parent.FindPropertyRelative (toGet);
        }
    }

    public static class SerializedPropertyHelpers {
        /// <summary>
        /// Get string representation of serialized property, even for non-string fields
        /// </summary>
        public static string AsStringValue (this SerializedProperty property) {
            switch (property.propertyType) {
                case SerializedPropertyType.String:
                    return property.stringValue;
                case SerializedPropertyType.Character:
                case SerializedPropertyType.Integer:
                    if (property.type == "char") return System.Convert.ToChar (property.intValue).ToString ();
                    return property.intValue.ToString ();
                case SerializedPropertyType.ObjectReference:
                    return property.objectReferenceValue != null ? property.objectReferenceValue.ToString () : "null";
                case SerializedPropertyType.Boolean:
                    return property.boolValue.ToString ();
                case SerializedPropertyType.Enum:
                    return property.enumNames[property.enumValueIndex];
                default:
                    return string.Empty;
            }
        }

    }
}