using UnityEngine;
using System.Collections;
using UnityEditor;

namespace KeiserSDK
{
    [CustomEditor(typeof(KeiserManager))]
    public class KeiserEditor : Editor
    {
        public override void OnInspectorGUI ()
        {
            DrawDefaultInspector ();
            KeiserManager script = (KeiserManager)target;
        
            EditorGUILayout.BeginHorizontal ();
        
            if (GUILayout.Button ("Start Listening")) {
                script.StartListener ();
            }
        
            if (GUILayout.Button ("Stop Listening", new GUILayoutOption[] {  })) {
                script.StopListener ();
            }
        
            EditorGUILayout.EndHorizontal ();
        }
    }
}