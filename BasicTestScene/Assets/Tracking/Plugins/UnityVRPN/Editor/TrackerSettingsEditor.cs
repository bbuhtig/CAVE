﻿using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[CustomEditor(typeof(TrackerSettings))]
public class TrackerSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TrackerSettings settings = target as TrackerSettings;

        if (settings != null)
        {
            settings.HostSettings = (TrackerHostSettings)EditorGUILayout.ObjectField("Host Settings", settings.HostSettings, typeof(TrackerHostSettings), true);
            settings.ObjectName = EditorGUILayout.TextField("Object Name", settings.ObjectName);
            settings.Channel = EditorGUILayout.IntField("Channel", settings.Channel);
            settings.TrackPosition = EditorGUILayout.Toggle("Track Position", settings.TrackPosition);
            settings.TrackRotation = EditorGUILayout.Toggle("Track Rotation", settings.TrackRotation);
			//added by pohl 9.12.16
			settings.Buttons = EditorGUILayout.IntField("Number of buttons", settings.Buttons);
			EditorGUILayout.PropertyField (this.serializedObject.FindProperty ("triggerButton"));
			EditorGUILayout.PropertyField (this.serializedObject.FindProperty ("leftButton"));
			EditorGUILayout.PropertyField (this.serializedObject.FindProperty ("rightButton"));
			EditorGUILayout.PropertyField (this.serializedObject.FindProperty ("middleButton"));
			this.serializedObject.ApplyModifiedProperties ();
			            

			if (GUI.changed)
            {
                EditorUtility.SetDirty(settings);
            }
        }
    }
}
