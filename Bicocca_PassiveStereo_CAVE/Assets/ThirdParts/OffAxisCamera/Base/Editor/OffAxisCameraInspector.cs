/*
	Created by Carl Emil Carlsen.
	Copyright 2017 Sixth Sensor.
	All rights reserved.
	http://sixthsensor.dk
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(OffAxisCamera))]
public class OffAxisCameraInspector : Editor
{
    SerializedProperty _wallTransform;


    void OnEnable()
    {
        _wallTransform = serializedObject.FindProperty("wall");

        // Ensure that the script will be executed late compared to other scripts.
        MonoScript script = MonoScript.FromMonoBehaviour(target as MonoBehaviour);

        if (MonoImporter.GetExecutionOrder(script) != 5000)
        {
            MonoImporter.SetExecutionOrder(script, 5000);
        }
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_wallTransform);
        
        if (_wallTransform.objectReferenceValue == null)
        {
            EditorGUILayout.HelpBox("Set WallTransform to activate.", MessageType.Warning);
        }

        serializedObject.ApplyModifiedProperties();
    }
}