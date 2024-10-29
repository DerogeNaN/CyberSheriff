#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SoundManager))]
public class SoundManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draw the default inspector

        SoundManager soundManager = (SoundManager)target;

        // Display code snippet for each sound
        foreach (var sound in soundManager.sounds)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Code Snippet for '{sound.name}'", EditorStyles.boldLabel);

            string codeSnippet = sound.soundType == SoundManager.SoundType.Global2D
                ? $"SoundManager.Instance.PlaySound(\"{sound.name}\");"
                : $"SoundManager.Instance.PlaySound(\"{sound.name}\", targetTransform);";

            EditorGUILayout.TextField("Code Snippet:", codeSnippet);
        }
    }
}
#endif