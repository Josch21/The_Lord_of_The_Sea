using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DeleteAllPrefabs))]
public class DeleteAllPrefabsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        if (GUILayout.Button("Delete All Prefabs"))
        {
            DeleteAllPrefabs script = (DeleteAllPrefabs)target;
            script.YourButtonClickMethod();
        }
    }
}