using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LocalizationWizard {
	
	public class LocalizationSettings : ScriptableObject {
		private const string FILE_NAME = "Settings";
		private const string SETTINGS_PATH = LocalizationWizardUtil.RESOURCES_FOLDER_PATH + "/" + FILE_NAME + ".asset";

		public SystemLanguage defaultLanguage = SystemLanguage.English;
		public List<LanguageState> LanguageStates = new List<LanguageState>();

		public static SystemLanguage UserLanguage {
			get {
				if (PlayerPrefs.HasKey (LocalizationWizardUtil.LANGUAGE_PREF_TAG))
					return (SystemLanguage)System.Enum.Parse (typeof(SystemLanguage), PlayerPrefs.GetString (LocalizationWizardUtil.LANGUAGE_PREF_TAG));
				else if (GetSettings ().Languages.Contains (Application.systemLanguage)) {
					PlayerPrefs.SetString (LocalizationWizardUtil.LANGUAGE_PREF_TAG, Application.systemLanguage.ToString ());
					return Application.systemLanguage;
				} else {
					PlayerPrefs.SetString (LocalizationWizardUtil.LANGUAGE_PREF_TAG, GetSettings ().defaultLanguage.ToString ());
					return GetSettings ().defaultLanguage;
				}
			}
			set {
				PlayerPrefs.SetString (LocalizationWizardUtil.LANGUAGE_PREF_TAG, value.ToString ());
				LocalizationManager.OnLanguageChanged (value);
			}
		}

		public List<SystemLanguage> Languages {
			get {
				List<SystemLanguage> result = new List<SystemLanguage> ();
				foreach (LanguageState s in LanguageStates)
					result.Add (s.language);
				return result;
			}
		}

		public Dictionary<SystemLanguage, Sprite> Icons {
			get {
				Dictionary<SystemLanguage, Sprite> result = new Dictionary<SystemLanguage, Sprite> ();
				foreach (LanguageState s in LanguageStates)
					result.Add (s.language, s.icon);
				return result;
			}
		}

		static LocalizationSettings settings = null;
		public static LocalizationSettings GetSettings() {
			if (settings == null)
				settings = ScriptableObjectUtil.Get<LocalizationSettings> (FILE_NAME);
			return settings;
		}

		#if UNITY_EDITOR
		[MenuItem ("LocalizationWizard/Settings")]
		public static void ShowOrCreateSettingsData()
		{
			Selection.activeObject = GetSettings ();
		}
		#endif

		[System.Serializable]
		public class LanguageState {
			public string name;
			public SystemLanguage language;
			public Sprite icon;
		}
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(LocalizationSettings))]
	public class LocalizationSettingsEditor : Editor {
		LocalizationSettings settings;

		Color defaultColor, defaultBackgroundColor;

		void OnEnable() {
			defaultColor = GUI.color;
			defaultBackgroundColor = GUI.backgroundColor;

			foreach (LocalizationSettings.LanguageState s in (target as LocalizationSettings).LanguageStates)
				if (string.IsNullOrEmpty (s.name))
					s.name = s.language.ToString ();
		}

		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();
			EditorGUI.BeginChangeCheck();

			settings = target as LocalizationSettings;

			GUILayout.Label ("Languages", EditorStyles.boldLabel);

			int indexToDelete = -1;
			for (int i = 0; i < settings.LanguageStates.Count; i++) {
				LocalizationSettings.LanguageState state = DrawState (settings.LanguageStates [i]);
				if (state == null)
					indexToDelete = i;
				else
					settings.LanguageStates [i] = state;
			}
			if (indexToDelete != -1)
				settings.LanguageStates.RemoveAt (indexToDelete);

			GUILayout.Space (5f);
			if (GUILayout.Button ("+", GUILayout.Width (40f), GUILayout.Height (40f))) {
				LocalizationSettings.LanguageState state = new LocalizationSettings.LanguageState ();
				state.language = SystemLanguage.Unknown;
				settings.LanguageStates.Add (state);
			}

			if(GUI.changed)
				UnityEditor.EditorUtility.SetDirty(target);

			EditorGUI.EndChangeCheck ();
			serializedObject.ApplyModifiedProperties ();
		}

		LocalizationSettings.LanguageState DrawState(LocalizationSettings.LanguageState state) {
			GUI.backgroundColor = state.language == settings.defaultLanguage ? Color.green : defaultBackgroundColor;
			EditorGUILayout.BeginVertical (GUI.skin.box);
			GUI.backgroundColor = defaultBackgroundColor;

			GUILayout.Space (5f);
			EditorGUILayout.BeginHorizontal ();
			state.language = (SystemLanguage) EditorGUILayout.EnumPopup (state.language);
			GUI.color = Color.red;
			if (GUILayout.Button ("X", GUILayout.Width (30f)))
				return null;
			GUI.color = defaultColor;
			EditorGUILayout.EndHorizontal ();
			state.name = EditorGUILayout.TextField ("Name", state.name);
			GUILayout.Space (5f);
			state.icon = EditorGUILayout.ObjectField ("Icon", state.icon, typeof(Sprite), false) as Sprite;
			GUILayout.Space (5f);

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label (" ");
			if (state.language != settings.defaultLanguage) {
				if (GUILayout.Button ("Make default", EditorStyles.toolbarButton, GUILayout.Width (100f)))
					settings.defaultLanguage = state.language;
			} else {
				GUILayout.Label ("Default", EditorStyles.boldLabel, GUILayout.Width (70f));
			}
			EditorGUILayout.EndHorizontal ();

			foreach (LocalizationSettings.LanguageState l in settings.LanguageStates) {
				if (l.language == state.language && l != state) {
					EditorGUILayout.HelpBox ("Duplicate language", MessageType.Error);
					break;
				}
			}

			GUILayout.Space (5f);
			EditorGUILayout.EndVertical ();
			return state;
		}
	}
	#endif

}