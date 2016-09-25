using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;



[CustomEditor (typeof(TileMap))]
public class TileMapEditor : Editor {


    private TileEditorState currentState;

    private TileSelectionState tileSelectionState;
    private TilePainterState tilePainterState;
    private TileRaiserState tileRaiserState;
    private TileMaterialState tileMaterialState;
    private TileRotateState tileRotateState;
    private TilePlaceObjectState tilePlaceObjectState;

    private Texture materialIcon;
    private Texture selectIcon;
    private Texture rotateIcon;
    private Texture placeIcon;
    private Texture raiseIcon;
    private Texture paintIcon;


    void OnEnable()
    {  


        tileSelectionState = new TileSelectionState(target as TileMap);
        tilePainterState = new TilePainterState();
        tileRaiserState = new TileRaiserState(target as TileMap);
        tileMaterialState = new TileMaterialState();
        tileRotateState = new TileRotateState();
        tilePlaceObjectState = new TilePlaceObjectState(target as TileMap);

        currentState = tileSelectionState;

        materialIcon = FindAssets.GetMaterialIcon();
        selectIcon = Resources.Load("Icons/select_icon") as Texture;
        rotateIcon = Resources.Load("Icons/rotate_icon") as Texture;
        placeIcon = Resources.Load("Icons/place_icon") as Texture;
        raiseIcon = Resources.Load("Icons/raise_icon") as Texture;
        paintIcon = Resources.Load("Icons/paint_icon") as Texture;

    }


   
   
    // Don't call e.Use() every frame or the scene view will never update until you manually click out and back in!!
    void OnSceneGUI()
    {
        Event e = Event.current;

        if (e.type == EventType.KeyDown)
        {
            OnKeyDown(e);
            e.Use();
            EditorSceneManager.MarkAllScenesDirty();
        }

        currentState.OnSceneGUI(Event.current);
       
        Repaint();   // Forces inspector to redraw. Useful for situations where inspector is modified outside of OnInspectorGUI

        
    }

 

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        gregbob.EditorUtility.CreateSectionLabel("Tile Editor");

        int iconSize = 48;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(selectIcon, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
        {
            ChangeState(tileSelectionState, currentState);
        }
        if (GUILayout.Button(materialIcon, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
        {
            ChangeState(tileMaterialState, currentState);
        }
        if (GUILayout.Button(rotateIcon, GUILayout.Width(iconSize), GUILayout.Height(iconSize))) {
            ChangeState(tileRotateState, currentState);
        }
        if (GUILayout.Button(placeIcon, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
        {
            ChangeState(tilePlaceObjectState, currentState);
        }
        if (GUILayout.Button(raiseIcon, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
        {
            ChangeState(tileRaiserState, currentState);
        }
        if (GUILayout.Button(paintIcon, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
        {
            ChangeState(tilePainterState, currentState);
        }
        GUILayout.EndHorizontal();

        currentState.OnGUI();

        if (GUI.changed)
            EditorUtility.SetDirty(target);
        
    }

    public void OnKeyDown(Event e)
    {
        if (e.keyCode == KeyCode.I)
        {
            ChangeState(tileSelectionState, currentState);
        }
        else if (e.keyCode == KeyCode.P)
        {
            ChangeState(tilePainterState, currentState);
        }
        else if (e.keyCode == KeyCode.R)
        {
            ChangeState(tileRaiserState, currentState);
        }
        else if (e.keyCode == KeyCode.T)
        {
            ChangeState(tileMaterialState, currentState);
        }
        else if (e.keyCode == KeyCode.O)
        {
            ChangeState(tileRotateState, currentState);
        }
        else if (e.keyCode == KeyCode.U)
        {
            ChangeState(tilePlaceObjectState, currentState);
        }

    }

    public void ChangeState(TileEditorState newState, TileEditorState currState)
    {
        currentState = newState;

    }

}
