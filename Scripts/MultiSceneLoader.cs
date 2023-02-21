using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.Utility
{
    public class MultiSceneLoader : MonoBehaviour
    {
        List<AsyncOperation> scenes = new List<AsyncOperation>();
        public bool isLoaded;

        public void SetScenesToLoad(MultiSceneData _sceneData)
        {
            isLoaded = false;
            foreach (var scene in _sceneData.scenes)
            {
                if (!SceneManager.GetSceneByName(scene.name).isLoaded) scenes.Add(SceneManager.LoadSceneAsync(scene.name, LoadSceneMode.Additive));
            }
            StartCoroutine(LoadScenes());
        }

        public void SetScenesToUnload(MultiSceneData _sceneData)
        {
            foreach (var scene in _sceneData.scenes)
            {
                if (SceneManager.GetSceneByName(scene.name).isLoaded) scenes.Add(SceneManager.UnloadSceneAsync(scene.name));
            }
            StartCoroutine(UnloadScenes());
        }

        public void SetSingleSceneToLoad(string sceneName)
        {
            if (!SceneManager.GetSceneByName(sceneName).isLoaded) scenes.Add(SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive));
            StartCoroutine(LoadScenes());
        }

        public void SetSingleSceneToUnload(string sceneName)
        {
            if (SceneManager.GetSceneByName(sceneName).isLoaded) 
                scenes.Add(SceneManager.UnloadSceneAsync(sceneName));
            StartCoroutine(UnloadScenes());
        }

        IEnumerator LoadScenes()
        {
            float totalProgress = 0f;
            for (int i = 0; i < scenes.Count; i++)
            {
                while (!scenes[i].isDone)
                {
                    totalProgress += scenes[i].progress;
                    float fillAmount = totalProgress/scenes.Count;
                    Debug.Log("Loading: " + fillAmount);
                    yield return null;
                }
                isLoaded = true;
                Debug.Log("Scenes loaded.");
            }
        }

        IEnumerator UnloadScenes()
        {
            float totalProgress = 0f;
            for (int i = 0; i < scenes.Count; i++)
            {
                while (!scenes[i].isDone)
                {
                    totalProgress += scenes[i].progress;
                    float fillAmount = totalProgress/scenes.Count;
                    Debug.Log("Loading: " + fillAmount);
                    yield return null;
                }

                Debug.Log("Scenes unloaded.");
            }
        }
    }
}