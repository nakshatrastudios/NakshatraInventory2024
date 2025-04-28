using UnityEditor;
using UnityEngine;

public abstract class InventoryManagerBaseEditor : EditorWindow
{
    protected Vector2 scrollPosition;

    protected void BeginScroll()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(position.width - 160), GUILayout.Height(position.height));
    }

    protected void EndScroll()
    {
        EditorGUILayout.EndScrollView();
    }
}
