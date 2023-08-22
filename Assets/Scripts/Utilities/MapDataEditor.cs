using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapData))]
public class MapDataEditor : Editor
{
    SerializedProperty _mapCreationType;
    SerializedProperty _mapWidth;
    SerializedProperty _mapHeight;
    SerializedProperty _textMap;
    SerializedProperty _textureMap;

    private void OnEnable()
    {
        _mapCreationType = serializedObject.FindProperty("_mapCreationType");
        _mapWidth = serializedObject.FindProperty("_mapWidth");
        _mapHeight = serializedObject.FindProperty("_mapHeight");
        _textureMap = serializedObject.FindProperty("_textureMap");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(_mapCreationType);

        if (_mapCreationType.enumValueIndex == 0)
        {
            EditorGUILayout.PropertyField(_mapWidth);
            EditorGUILayout.PropertyField(_mapHeight);
        }
        else
        {
            EditorGUILayout.PropertyField(_textureMap);
        }

        serializedObject.ApplyModifiedProperties();

        base.OnInspectorGUI();
    }
}