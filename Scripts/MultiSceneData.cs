using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.Utility
{
    [CreateAssetMenu(fileName = "New MultiScene Data", menuName = "Utility/MultiScene/MultiScene Data")]
    public class MultiSceneData : ScriptableObject
    {
        //public List<string> scenes;
        public List<SceneAsset> scenes;
    }
}