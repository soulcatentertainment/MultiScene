using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEditor.SearchService;
using System.Collections.Generic;

namespace Runtime.Utility
{
    public class MultiSceneEditorLoader : EditorWindow
    {
        MultiSceneData sceneData;

        [MenuItem("Gameplay/MultiScene/Load Main scene")]
        public static void OpenMainScene()
        {
            //Open the Scene in the Editor (do not enter Play Mode)
            string[] guids = AssetDatabase.FindAssets("t:Scene Main");
            if (guids.Length == 0)
            {
                Debug.LogWarning("Couldn't find Main scene");
                return;
            }
            string scenePath = AssetDatabase.GUIDToAssetPath(guids[0]);
            EditorSceneManager.OpenScene(scenePath);
        }

        [MenuItem("Gameplay/MultiScene/Load MultiScene Data")]
        public static void LoadMSceneData()
        {
            //Get existing window or create new one
            MultiSceneEditorLoader window = (MultiSceneEditorLoader)EditorWindow.GetWindow(typeof(MultiSceneEditorLoader));
            window.titleContent = new GUIContent("Load MultiScene");
            window.Show();
        }

        void LoadMultipleScene(MultiSceneData mSceneData)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                // Iterate and load each scene
                foreach (var scene in mSceneData.scenes)
                {
                    string scenePath = AssetDatabase.GetAssetPath(scene);
                    EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                }

                Debug.Log("MultiScenes loaded sucessfully.");
            }
        }

        [MenuItem("Gameplay/MultiScene/Load All MultiScenes to Build")]
        public static void LoadAllMultiScenesToBuild()
        {
            List<EditorBuildSettingsScene> scenesToBuild = new List<EditorBuildSettingsScene>();

            // Add Main scene to build
            string[] mainSceneguids = AssetDatabase.FindAssets("t:Scene Main");
            if (mainSceneguids.Length == 0)
            {
                Debug.LogWarning("Couldn't find Main scene");
                return;
            }

            string mainScenePath = AssetDatabase.GUIDToAssetPath(mainSceneguids[0]);
            scenesToBuild.Add(new EditorBuildSettingsScene(mainScenePath, true));

            // Get all MultiScene assets in the folder
            string[] multiSceneGUIDs = AssetDatabase.FindAssets("t:MultiSceneData");

            // Get scenes from multiScenes and add to build
            for (int i = 0; i < multiSceneGUIDs.Length; i++)
            {
                string multiScenePath = AssetDatabase.GUIDToAssetPath(multiSceneGUIDs[i]);
                MultiSceneData instance = (MultiSceneData)AssetDatabase.LoadAssetAtPath(multiScenePath, typeof(MultiSceneData));

                // Iterate over the scenes in the MultiSceneData
                foreach (var scene in instance.scenes)
                {
                    string scenePath = AssetDatabase.GetAssetPath(scene);
                    scenesToBuild.Add(new EditorBuildSettingsScene(scenePath, true));
                }
            }

            EditorBuildSettings.scenes = scenesToBuild.ToArray();
        }

        private void OnGUI()
        {
            // GUI Layout
            GUILayout.Label("Select MultiScene Data:", EditorStyles.boldLabel);
            sceneData = EditorGUILayout.ObjectField("MultiScene Data", sceneData, typeof(MultiSceneData), true) as MultiSceneData;

            if (GUILayout.Button("Load"))
            {
                OpenMainScene();                // Load main scene first
                LoadMultipleScene(sceneData);   // then load the scenes from MScene data
            }
        }

        public static class SceneLoaderUtility
        {
            static string sceneToOpen;

            public static void LoadScene(string sceneName)
            {
                if (EditorApplication.isPlaying)
                {
                    EditorApplication.isPlaying = false;
                }

                sceneToOpen = sceneName;
                EditorApplication.update += OnUpdate;
            }

            static void OnUpdate()
            {
                if (sceneToOpen == null ||
                    EditorApplication.isPlaying || EditorApplication.isPaused ||
                    EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    return;
                }

                EditorApplication.update -= OnUpdate;

                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    // need to get scene via search because the path to the scene
                    // file contains the package version so it'll change over time
                    string[] guids = AssetDatabase.FindAssets("t:scene " + sceneToOpen, null);
                    if (guids.Length == 0) { Debug.LogWarning("Couldn't find scene file"); }
                    else
                    {
                        string scenePath = AssetDatabase.GUIDToAssetPath(guids[0]);
                        EditorSceneManager.OpenScene(scenePath);
                    }
                }
                sceneToOpen = null;
            }
        }
    }
}