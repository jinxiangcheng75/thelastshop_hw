using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class GUILayoutFileEditor : Editor
{
    [MenuItem("Assets/Test")]
    public static void SelectionTest()
    {
        string[] guids = Selection.assetGUIDs;
        for (int i = 0; i < guids.Length; i++)
        {
            Debug.Log(string.Format("{0} : path = {1}", guids[i], AssetDatabase.GUIDToAssetPath(guids[i])));
        }
    }
}
