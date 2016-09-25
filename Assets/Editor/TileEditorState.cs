using UnityEngine;
using System.Collections;
using UnityEditor;

public class TileEditorState
{

    //	public virtual void OnInspectorGUI(Event e)
    //    {
    //        if (e.type == EventType.MouseDrag || e.type == EventType.MouseDown)
    //        {
    //            RaycastHit hit;

    //            var ray = HandleUtility.GUIPointToWorldRay(new Vector3(e.mousePosition.x, e.mousePosition.y, Camera.main.transform.position.z * -1));

    //            if (Physics.Raycast(ray, out hit))
    //            {

    //                if (state == EditorState.ADD_REMOVE)
    //                {
    //                    if (e.button == 0)
    //                    {
    //                        tileSelection.Select(tile);

    //                    }
    //                    else if (e.button == 1)
    //                    {
    //                        tileSelection.Deselect(tile);
    //                    }


    //                }
    //            }
    //            e.Use();
    //            //EditorSceneManager.MarkAllScenesDirty();        // Allows scene to be saved. Need a better fix.
    //        }
    //    }

    //    public void GetClickedOnTile()
    //    {

    //    }

}