using UnityEngine;
using UnityEditor;
using UnityToolbarExtender;
using Runtime.Utility;
using UnityEditor.SceneManagement;
using System;

[InitializeOnLoad]
public static class SceneToolbar
{
	[InitializeOnLoad]
	public class SceneSwitchLeftButton
	{
		static SceneSwitchLeftButton()
		{
			ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
		}

		static Texture GetIcon(string iconName) => Resources.Load<Texture>(iconName);
		static void OnToolbarGUI()
		{
			GUILayout.FlexibleSpace();

			GUIStyle buttonStyle = new GUIStyle("Command")
			{
				fontSize = 5,
				alignment = TextAnchor.MiddleCenter,
				imagePosition = ImagePosition.ImageAbove,
				fontStyle = FontStyle.Bold,
			};

			if (GUILayout.Button(new GUIContent(GetIcon("ic-mainscene-button"), "Start from Main"), buttonStyle))
			{
				MultiSceneEditorLoader.SceneLoaderUtility.LoadScene("Main");
			}

			if (GUILayout.Button(new GUIContent(GetIcon("ic-multiscene-data"), "Load Level from MScene"), buttonStyle))
			{
				MultiSceneEditorLoader.LoadMSceneData();
			}
		}
	}
}
