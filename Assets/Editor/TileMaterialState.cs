using UnityEngine;
using System.Collections;
using UnityEditor;

public class TileMaterialState : TileEditorState {

    private ResizableSelectionWindow materialSelection;
    private Material[] allMaterials;
    private Material paintMaterial;
    private int iconSize = 64;


    public TileMaterialState()
    {
        allMaterials = System.Array.ConvertAll(FindAssets.FindAllAssets("material"), item => (Material)item);
        materialSelection = new ResizableSelectionWindow(iconSize);
        materialSelection.onSelection += MaterialSelection;
    }

    public void MaterialSelection(object sender, OnSelectionEventArgs e)
    {
        paintMaterial = (Material)e.clicked;
    }

    public override void OnGUI()
    {
        gregbob.EditorUtility.CreateSectionLabel("Add material to tile");
        EditorGUILayout.ObjectField("Texture to paint", paintMaterial, typeof(Material), true);
        materialSelection.CreateResizableBox(allMaterials);
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
                    if (e.control)
                    {
                        paintMaterial = clickedOn.GetComponent<Renderer>().sharedMaterial;
                    }
                    else
                    {
                        clickedOn.GetComponent<Renderer>().material = paintMaterial;

                    }
                }
            }
            e.Use();
            //EditorSceneManager.MarkAllScenesDirty();        // Allows scene to be saved. Need a better fix.
        }

    }
}
