using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AbstractDungeonGenerator), true)]
public class RandomDungeonGeneratorEditor : Editor
{
    AbstractDungeonGenerator generator;

    private void Awake()
    {
        generator = (AbstractDungeonGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Create Dungeon"))
        {
            generator.GenerateDungeon();
        }
        if (GUILayout.Button("Clear Node"))
        {
            GameObject parent = GameObject.Find("NodeParent");

            if (parent != null)
            {
                for (int i = parent.transform.childCount - 1; i >= 0; i--)
                {
                    if (parent.transform.GetChild(i).gameObject != null)
                    {
                        DestroyImmediate(parent.transform.GetChild(i).gameObject);
                    }
                }
                Debug.Log("All child nodes cleared.");
            }
            else
            {
                Debug.LogWarning("NodeParent not found!");
            }
        }
    }
}
