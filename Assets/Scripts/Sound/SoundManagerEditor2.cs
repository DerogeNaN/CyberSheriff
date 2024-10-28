#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SoundManager2))]
public class SoundManagerEditor2 : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draw the default inspector

        SoundManager2 soundManager = (SoundManager2)target;

        // Display code snippet for each sound
        foreach (var sound in soundManager.sounds)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Code Snippet for Sound '{sound.name}'", EditorStyles.boldLabel);
            string codeSnippet = sound.soundType == SoundManager2.SoundType.Global2D
                ? $"SoundManager2.Instance.PlaySound(\"{sound.name}\");"
                : $"SoundManager2.Instance.PlaySound(\"{sound.name}\", targetTransform);";

            EditorGUILayout.TextField("Code Snippet:", codeSnippet);
        }

        // Display code snippet for each music track
        foreach (var track in soundManager.musicTracks)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Code Snippet for Music '{track.name}'", EditorStyles.boldLabel);
            string codeSnippet = $"SoundManager2.Instance.PlayMusic(\"{track.name}\");";
            EditorGUILayout.TextField("Code Snippet:", codeSnippet);
        }

        // Display code snippet for each ambience clip
        foreach (var ambience in soundManager.ambienceClips)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Code Snippet for Ambience '{ambience.name}'", EditorStyles.boldLabel);
            string codeSnippet = $"SoundManager2.Instance.PlayAmbience(\"{ambience.name}\");";
            EditorGUILayout.TextField("Code Snippet:", codeSnippet);
        }
    }
}
#endif