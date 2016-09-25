using UnityEngine;
using System.Collections;
using UnityEditor;

public class TilePainter  {

    public Color paintColor = Color.white;
    public Color paintSecondaryColor = Color.black;

    public void OnGUI()
    {
        GUILayout.BeginHorizontal();
        paintColor = EditorGUILayout.ColorField("Primary", paintColor);
        paintSecondaryColor = EditorGUILayout.ColorField("Secondary", paintSecondaryColor);
        GUILayout.EndHorizontal();
    }
}
