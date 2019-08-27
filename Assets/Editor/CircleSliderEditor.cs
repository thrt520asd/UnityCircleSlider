using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(CircleSlider) , true)]
public class CircleSliderEditor : SelectableEditor
{
    private SerializedProperty m_Value;
    private SerializedProperty FillOrigin;
    private SerializedProperty m_MaxValue;
    private SerializedProperty m_MinValue;
    private SerializedProperty m_FillRect;
    private SerializedProperty m_HandleRect;
    private SerializedProperty m_FillOrigin;
    private SerializedProperty m_Radius;
    private SerializedProperty m_ClockWise;
//    private SerializedProperty m_MaxAngle;
//    private SerializedProperty m_MinAngle;
    private SerializedProperty m_WholeNumbers;


    protected override void OnEnable()
    {
        base.OnEnable();
        
        m_FillRect = serializedObject.FindProperty("m_FillRect");
        m_HandleRect = serializedObject.FindProperty("m_HandleRect");
        m_MinValue = serializedObject.FindProperty("m_MinValue");
        m_MaxValue = serializedObject.FindProperty("m_MaxValue");
        m_Value = serializedObject.FindProperty("m_Value");
        m_FillOrigin = serializedObject.FindProperty("m_FillOrigin");
        m_Radius = serializedObject.FindProperty("m_Radius");
        m_ClockWise = serializedObject.FindProperty("m_ClockWise");
        m_WholeNumbers = serializedObject.FindProperty("m_WholeNumbers");
//        m_MaxAngle = serializedObject.FindProperty("m_MaxAngle");
//        m_MinAngle = serializedObject.FindProperty("m_MinAngle");
        
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        EditorGUILayout.PropertyField(m_FillRect);
        EditorGUILayout.PropertyField(m_HandleRect);
        
        if (m_FillRect.objectReferenceValue != null )
        {
            EditorGUILayout.PropertyField(m_MinValue);
            EditorGUILayout.PropertyField(m_MaxValue);
            EditorGUILayout.PropertyField(m_WholeNumbers);
            EditorGUILayout.Slider(m_Value, m_MinValue.floatValue, m_MaxValue.floatValue);
            EditorGUILayout.PropertyField(m_FillOrigin);
            EditorGUILayout.PropertyField(m_ClockWise);
        }

        if (m_HandleRect.objectReferenceValue != null)
        {
            EditorGUILayout.PropertyField(m_Radius);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
