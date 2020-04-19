using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Runtime.InteropServices;

/// <summary>
/// An editor script that replaces all selected GameObjects in the scene with the given prefab
/// </summary>
public class ReplaceWithPrefab : EditorWindow
{
    [SerializeField]
    private GameObject prefab;
    
    [MenuItem("Tools/Replace With Prefab")]
    static void CreateReplaceWithWindow()
    {
        EditorWindow.GetWindow<ReplaceWithPrefab>();

    }

    private void OnGUI()
    {
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

        if (GUILayout.Button("Replace"))
        {
            if(prefab != null)
            {
                GameObject[] selected = Selection.gameObjects;
                PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(prefab);
                GameObject newObject;
                
                foreach (GameObject go in selected)
                {
                    //Replace each GameObject go with the prefab positioned and rotated the same as the object was
                    //The new object also has the same parent object as the previous object and the same sibling index (order in parent's hierarchy)
                    
                    if (prefabType == PrefabAssetType.NotAPrefab)
                    {
                        //Object is not a prefab, just instantiate it
                        newObject = Instantiate(prefab);
                        newObject.name = prefab.name;
                    }
                    else
                    {
                        //The object is a prefab, instantiate it as an instance of one
                        newObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                    }

                    if(newObject == null)
                    {
                        Debug.Log("Error instantiating prefab!");
                    }

                    Undo.RegisterCreatedObjectUndo(newObject, "Replace With Prefab");
                    newObject.transform.parent = go.transform.parent;
                    newObject.transform.localPosition = go.transform.localPosition;
                    newObject.transform.localRotation = go.transform.localRotation;
                    newObject.transform.SetSiblingIndex(go.transform.GetSiblingIndex());

                    
                     Undo.DestroyObjectImmediate(go);
                }
            }
            else
            {
                Debug.Log("Error: No prefab selected!");
            }
           

        }

        EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);

    }

}
