using UnityEngine;
using System.Collections;

public struct OnSelectionEventArgs
{

    public object clicked;
    public Texture2D icon;

    public OnSelectionEventArgs(object clicked, Texture2D icon)
    {
        this.clicked = clicked;
        this.icon = icon;
    }
}

public class ResizableSelectionWindow  {

   

    private int iconSize = 64;

    public delegate void OnSelection(object sender, OnSelectionEventArgs e);
    public event OnSelection onSelection;

    private Vector2 scrollPos;

    public ResizableSelectionWindow (int iconSize)
    {
        this.iconSize = iconSize;
    }



    public void CreateResizableBox(Object[] objs)
    {

        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true), GUILayout.MaxHeight(iconSize * 3), GUILayout.MinHeight(iconSize));

        int textHeight = 15;
        int boxWidth = Screen.width;
        int numTextures = objs.Length;
        int numTilesInRow = boxWidth / iconSize;

        int numRows = (numTextures + numTilesInRow - 1) / numTilesInRow; // Rounded up integer division
        int boxHeight = numRows * iconSize;
        int boxPadding = 0;

        GUILayout.Box("", GUILayout.Height(boxHeight));

        int count = 0;                          // Decides index in the row
        int row = 0;                            // Decides which row
        Debug.Log(numTilesInRow);
        for (int i = 0; i < numTextures; i++)
        {
            Texture2D icon = UnityEditor.AssetPreview.GetAssetPreview(objs[i]);
            if (GUI.Button(new Rect(count * iconSize, row * iconSize + boxPadding, iconSize, iconSize), icon))
            {
                onSelection(this, CreateOnSelectionEvent(objs[i], icon));
            }
            GUI.TextArea(new Rect(count * iconSize, row * iconSize + boxPadding, iconSize, textHeight),objs[i].ToString());
            count++;
            if (count >= numTilesInRow)
            {
                count = 0;
                row += 1;
            }
        }
        GUILayout.EndScrollView();
    }

    private OnSelectionEventArgs CreateOnSelectionEvent(object clicked, Texture2D icon)
    {
        OnSelectionEventArgs e = new OnSelectionEventArgs(clicked, icon);

        return e;
    }

}
