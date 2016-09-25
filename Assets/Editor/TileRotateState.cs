using UnityEngine;
using System.Collections;
using UnityEditor;

public class TileRotateState : TileEditorState {

    float rotateBy = 90;

    public override void OnGUI()
    {
        gregbob.EditorUtility.CreateSectionLabel("Rotate");
        rotateBy = EditorGUILayout.FloatField("Rotate by: ", rotateBy);
    }


    public override void OnSceneGUI(Event e)
    {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));

        if (e.type == EventType.MouseDrag || e.type == EventType.MouseDown)
        {
            RaycastHit hit;

            var ray = HandleUtility.GUIPointToWorldRay(new Vector3(e.mousePosition.x, e.mousePosition.y, Camera.main.transform.position.z * -1));

            if (Physics.Raycast(ray, out hit))
            {
                hit.collider.transform.Rotate(hit.collider.transform.up, rotateBy);
            }
            e.Use();
            //EditorSceneManager.MarkAllScenesDirty();        // Allows scene to be saved. Need a better fix.
        }

    }
}
