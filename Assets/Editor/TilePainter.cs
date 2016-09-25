using UnityEngine;
using System.Collections;
using UnityEditor;

public class TilePainter  {

    public Color paintColor = Color.white;
    public Color paintSecondaryColor = Color.black;


    public void OnGUI()
    {
        paintColor = EditorGUILayout.ColorField("Primary", paintColor);
        paintSecondaryColor = EditorGUILayout.ColorField("Secondary", paintSecondaryColor);
    }
}
