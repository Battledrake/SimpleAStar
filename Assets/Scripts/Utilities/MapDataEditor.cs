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

    SerializedProperty _blockedColor;
    SerializedProperty _openColor;
    SerializedProperty _lightTerrainColor;
    SerializedProperty _mediumTerrainColor;
    SerializedProperty _heavyTerrainColor;

    private void OnEnable()
    {
        _mapCreationType = serializedObject.FindProperty("_mapCreationType");
        _mapWidth = serializedObject.FindProperty("_mapWidth");
        _mapHeight = serializedObject.FindProperty("_mapHeight");
        _textMap = serializedObject.FindProperty("_textMap");
        _textureMap = serializedObject.FindProperty("_textureMap");
        _blockedColor = serializedObject.FindProperty("_blockedColor");
        _openColor = serializedObject.FindProperty("_openColor");
        _lightTerrainColor = serializedObject.FindProperty("_lightTerrainColor");
        _mediumTerrainColor = serializedObject.FindProperty("_mediumTerrainColor");
        _heavyTerrainColor = serializedObject.FindProperty("_heavyTerrainColor");

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
        else if (_mapCreationType.enumValueIndex == 1)
        {
            //EditorGUILayout.PropertyField(_textMap);
            EditorGUILayout.ObjectField(_textMap, typeof(TextAsset));
        }
        else
        {
            EditorGUILayout.PropertyField(_textureMap);
            EditorGUILayout.PropertyField(_blockedColor);
            EditorGUILayout.PropertyField(_openColor);
            EditorGUILayout.PropertyField(_lightTerrainColor);
            EditorGUILayout.PropertyField(_mediumTerrainColor);
            EditorGUILayout.PropertyField(_heavyTerrainColor);
        }

        serializedObject.ApplyModifiedProperties();

        base.OnInspectorGUI();
    }
}