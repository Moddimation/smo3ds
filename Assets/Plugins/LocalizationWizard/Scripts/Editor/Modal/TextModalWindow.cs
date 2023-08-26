using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;

public class TextModalWindow : ModalWindow<string> {
	protected override void DrawGUI ()
	{
		input = EditorGUILayout.TextField (input);
		GUILayout.Space (10f);
		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Save", GUILayout.Width(50f)))
			SendResult (input);
		if (GUILayout.Button ("Close", GUILayout.Width(50f)))
			Close ();
		EditorGUILayout.EndHorizontal ();
	}
}
#endif
