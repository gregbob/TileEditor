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

    public ResizableSelectionWindow (int iconSize)
    {
        this.iconSize = iconSize;
    }



    public void CreateResizableBox(Vector2 position, Object[] objs)
    {

        //   Vector2 guiAbove = GUILayoutUtility.GetLastRect().position;
        // Width of current window. Gives width of inspector.

        int textHeight = 15;
        int boxWidth = Screen.width;
        int numTextures = objs.Length;
        int numTilesInRow = boxWidth / iconSize;

        int numRows = (numTextures + numTextures - 1) / numTilesInRow; // Rounded up integer division
        int boxHeight = numRows * iconSize;
        int boxPadding = 20;

        GUIStyle boxStyle = new GUIStyle();
        boxStyle.padding = new RectOffset(0, 0, 10, 10);

        GUIStyle g = new GUIStyle();
        g.padding = new RectOffset(5, 5, 10, 0);
        GUI.Box(new Rect(position.x, position.y + boxPadding, boxWidth - position.x * 2, boxHeight), "");

        int count = 0;                          // Decides index in the row
        int row = 0;                            // Decides which row
        for (int i = 0; i < numTextures; i++)
        {

            Texture2D icon = UnityEditor.AssetPreview.GetAssetPreview(objs[i]);
            if (GUI.Button(new Rect(count * iconSize + position.x, row * iconSize + position.y + boxPadding, iconSize, iconSize), icon, g))
            {
                onSelection(this, CreateOnSelectionEvent(objs[i], icon));
            }
            GUI.TextArea(new Rect(count * iconSize + position.x, row * iconSize + position.y + boxPadding, iconSize, textHeight),objs[i].ToString());
            count++;
            if (count >= numTilesInRow)
            {
                count = 0;
                row += 1;
            }
        }

    }

    private OnSelectionEventArgs CreateOnSelectionEvent(object clicked, Texture2D icon)
    {
        OnSelectionEventArgs e = new OnSelectionEventArgs(clicked, icon);

        return e;
    }

}
