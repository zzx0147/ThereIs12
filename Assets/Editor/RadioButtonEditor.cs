using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;
using UnityEditor;

[CustomEditor(typeof(RadioButton))]
public class RadioButtonEditor : ButtonEditor
{
    SerializedProperty m_OtherRadioButton;
    SerializedProperty m_ChangeDeltaOnPressed;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_OtherRadioButton = serializedObject.FindProperty("m_OtherButton");
        m_ChangeDeltaOnPressed = serializedObject.FindProperty("m_ChangeDeltaOnPressed");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_OtherRadioButton, new GUIContent("OtherRadioButton"));
        EditorGUILayout.PropertyField(m_ChangeDeltaOnPressed, new GUIContent("ChangeDeltaOnPressed"));
        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
    }
}
