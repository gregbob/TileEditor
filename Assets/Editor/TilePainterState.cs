using UnityEngine;
using System.Collections;
using UnityEditor;

public class TilePainterState : TileEditorState {

    TilePainter tilePainter;

    public TilePainterState(TilePainter tilePainter)
    {
        this.tilePainter = tilePainter;
    }

    public void OnSceneGUI(Event e)
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
                            tilePainter.paintColor = clickedOn.GetComponent<Renderer>().sharedMaterial.color;
                        }
                        gregbob.EditorUtility.ChangeColor(clickedOn.GetComponent<Renderer>(), tilePainter.paintColor);
                    }
                    else if (e.button == 1)
                    {
                        if (e.control)
                        {
                            tilePainter.paintSecondaryColor = clickedOn.GetComponent<Renderer>().sharedMaterial.color;
                        }
                        gregbob.EditorUtility.ChangeColor(clickedOn.GetComponent<Renderer>(), tilePainter.paintSecondaryColor);
                    }


                }
            }
            e.Use();
            //EditorSceneManager.MarkAllScenesDirty();        // Allows scene to be saved. Need a better fix.
        }
       
    }
}
