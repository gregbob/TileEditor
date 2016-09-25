using UnityEngine;
using System.Collections;
using UnityEditor;

public class TilePainterState : TileEditorState {

    Color paintColor = Color.white;
    Color paintSecondaryColor = Color.black;

    public TilePainterState()
    {

    }

    public override void OnGUI()
    {
        gregbob.EditorUtility.CreateSectionLabel("Paint tiles");
        GUILayout.BeginHorizontal();
        paintColor = EditorGUILayout.ColorField("Primary", paintColor);
        paintSecondaryColor = EditorGUILayout.ColorField("Secondary", paintSecondaryColor);
        GUILayout.EndHorizontal();
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
                GameObject clickedOn = hit.collider.gameObject;
                if (clickedOn != null)
                {
                    if (e.button == 0)
                    {
                        if (e.control)
                        {
                           paintColor = clickedOn.GetComponent<Renderer>().sharedMaterial.color;
                        }
                        gregbob.EditorUtility.ChangeColor(clickedOn.GetComponent<Renderer>(), paintColor);
                    }
                    else if (e.button == 1)
                    {
                        if (e.control)
                        {
                            paintSecondaryColor = clickedOn.GetComponent<Renderer>().sharedMaterial.color;
                        }
                        gregbob.EditorUtility.ChangeColor(clickedOn.GetComponent<Renderer>(), paintSecondaryColor);
                    }


                }
            }
            e.Use();
            //EditorSceneManager.MarkAllScenesDirty();        // Allows scene to be saved. Need a better fix.
        }
       
    }
}
