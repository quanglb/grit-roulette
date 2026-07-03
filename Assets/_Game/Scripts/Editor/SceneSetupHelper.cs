#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SceneSetupHelper : EditorWindow
{
    [MenuItem("Tools/Drinking Game/Setup Scenes")]
    public static void CreateAllScenes()
    {
        string[] scenes = {
            "MainMenuScene", "SampleScene", "BombPassScene", 
            "LuckyNumberScene", "SpinBottleScene", "ReactionDuelScene", 
            "HotPotatoScene", "HigherLowerScene", "DiceBattleScene", 
            "SecretVoteScene", "ColorTrapScene", "MemoryChainScene"
        };

        string sceneDir = "Assets/Scenes";
        if (!Directory.Exists(sceneDir))
        {
            Directory.CreateDirectory(sceneDir);
        }

        List<EditorBuildSettingsScene> buildScenes = new List<EditorBuildSettingsScene>();

        foreach (string sceneName in scenes)
        {
            string path = $"{sceneDir}/{sceneName}.unity";
            if (!File.Exists(path))
            {
                var newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
                EditorSceneManager.SaveScene(newScene, path);
                Debug.Log($"Created Scene: {path}");
            }
            buildScenes.Add(new EditorBuildSettingsScene(path, true));
        }

        EditorBuildSettings.scenes = buildScenes.ToArray();
        Debug.Log("Registered all scenes in Build Settings.");
        AssetDatabase.Refresh();
    }
}
#endif
