using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using System;
using System.IO;

[InitializeOnLoad]
public class BuildDateGenerator : IPreprocessBuild {
	static bool updated = false;

	static BuildDateGenerator() {
		// Called once on domain reload
		EditorApplication.update += OnEditorUpdate;
	}

	public int callbackOrder { get { return 0; } }

	public void OnPreprocessBuild(BuildTarget target, string path) {
		WriteBuildDate();
	}

	static void OnEditorUpdate() {
		// Detect entering play mode in Unity 5.6.6
		if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying && !updated) {
			updated = true;
			WriteBuildDate();
		}
		if (!EditorApplication.isPlayingOrWillChangePlaymode) {
			updated = false;
		}
	}

	static void WriteBuildDate() {
		string path = "Assets/Scripts/BuildDate.cs";
		string date = DateTime.Now.ToString("MMM dd yyyy HH:mm:ss");
		string content =
			"public static class BuildDate {\n" +
			"    public const string Date = \"" + date + "\";\n" +
			"}";
		if (File.Exists(path) && File.ReadAllText(path) == content)
			return;

		File.WriteAllText(path, content);
		AssetDatabase.Refresh();
		Debug.Log("BuildDate.cs updated: " + date);
	}
}
