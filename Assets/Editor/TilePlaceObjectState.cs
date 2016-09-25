using UnityEngine;
using System.Collections;
using UnityEditor;

public class TilePlaceObjectState : TileEditorState
{

    private ResizableSelectionWindow prefabSelection;
    private GameObject[] allPrefabs;
    private Material paintMaterial;
    private int iconSize = 64;

    private GameObject objToPlace;
    private GameObject objToPlacePreview;
    private bool visitableOnPlace = true;

    private TileMap tileMap;


    public TilePlaceObjectState(TileMap tileMap)
    {
        allPrefabs = System.Array.ConvertAll(FindAssets.FindAllAssets("prefab"), item => (GameObject)item);
        prefabSelection = new ResizableSelectionWindow(64);
        prefabSelection.onSelection += PrefabSelection;
        this.tileMap = tileMap;
    }

    public void MaterialSelection(object sender, OnSelectionEventArgs e)
    {
        paintMaterial = (Material)e.clicked;
    }

    public override void OnGUI()
    {
        gregbob.EditorUtility.CreateSectionLabel("Place object on top of tile");
        objToPlace = (GameObject)EditorGUILayout.ObjectField("Object to place", objToPlace, typeof(GameObject), true);
        visitableOnPlace = EditorGUILayout.Toggle("Tile visitable after placing object?", visitableOnPlace);
        prefabSelection.CreateResizableBox(allPrefabs);
    }

    public override void OnSceneGUI(Event e)
    {
        // Get controlID to prevent clicking to deselect the selected object in hierarchy
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
        if (e.type == EventType.MouseMove)
        {
            RaycastHit hit;
            if (Physics.Raycast(HandleUtility.GUIPointToWorldRay(e.mousePosition), out hit))
            {

                if (objToPlacePreview != null)
                    objToPlacePreview.transform.position = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y + 1, hit.collider.transform.position.z);
            }
            e.Use();
        }
        if (e.type == EventType.MouseDrag || e.type == EventType.MouseDown)
        {
            RaycastHit hit;

            var ray = HandleUtility.GUIPointToWorldRay(new Vector3(e.mousePosition.x, e.mousePosition.y, Camera.main.transform.position.z * -1));

            if (Physics.Raycast(ray, out hit))
            {
                Tile tile = Tile.GetTile(hit.collider.gameObject);
                if (e.button == 0)
                    tile.PlaceObject(objToPlace, visitableOnPlace);
                else if (e.button == 1)
                    tile.RemoveObject();
            }
            e.Use();
            UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();        // Allows scene to be saved. Need a better fix.
        }

    }

    public void PrefabSelection(object sender, OnSelectionEventArgs e)
    {

        objToPlace = (GameObject)e.clicked;
        if (objToPlacePreview != null)
            GameObject.DestroyImmediate(objToPlacePreview);

        objToPlacePreview = GameObject.Instantiate(objToPlace);
        objToPlacePreview.GetComponent<Collider>().enabled = false;

        objToPlacePreview.transform.SetParent(tileMap.transform);
    }
}
