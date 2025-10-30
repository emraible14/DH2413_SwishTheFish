using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(Pathing), true)]
public class PathingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        _ = this.DrawDefaultInspector(); // Draw the default inspector for the target script

        var script = (Pathing)this.target;

        GUILayout.Space(10);

        EditorGUILayout.LabelField("Number of Waypoints: " + script.Points.Count.ToString());

        if (GUILayout.Button("Delete last Waypoint"))
        {
            script.SubtractWaypoint();
        }

        if (GUILayout.Button("Add new Waypoint"))
        {
            script.AddWaypoint();
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Delete All Waypoints"))
        {
            script.DeleteAllWaypoints();
        }
    }
}
