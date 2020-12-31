using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Serializes the ResourceAmount data structure to be editable in the editor
/// </summary>
[CustomPropertyDrawer(typeof(ResourceAmount))]
public class ResourceAmountDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);
        Rect newRect = EditorGUI.PrefixLabel(position, label);
        newRect.width *= 0.5f;
        EditorGUI.indentLevel = 0;
        EditorGUI.PropertyField(newRect, property.FindPropertyRelative("type"), GUIContent.none);
        newRect.x += newRect.width;
        EditorGUIUtility.labelWidth = 25;

        EditorGUI.PropertyField(newRect, property.FindPropertyRelative("amount"), new GUIContent("Am."));

        EditorGUI.EndProperty();
 

    }

}
