using UnityEditor;
using UnityEngine;
using yutokun;

[CustomEditor(typeof(TextImporter))]
public class TextImporterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TextImporter textImporter = (TextImporter)target;
        if (GUILayout.Button("ImportText"))
        {
            textImporter.ImportText();
        }
    }
}
