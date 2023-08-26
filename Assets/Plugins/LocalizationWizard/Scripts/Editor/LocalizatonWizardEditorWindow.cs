using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
namespace LocalizationWizard {
	
	public class LocalizatonWizardEditorWindow: EditorWindow  {

		protected static ILocalizationManager manager;
		protected LocalizationSettings settings;

		protected LocalizedXmlData xmlData;

		Vector2 scrollPos;
		protected Color defColor, defContentColor, defBackgroundColor;
		protected bool changed = false;
		string valuesFilter = string.Empty;

		public static float CollumnWidth {
			get { return EditorPrefs.GetFloat ("localization_wizard_collumn_width", 120f); }
			set { EditorPrefs.SetFloat ("localization_wizard_collumn_width", value); }
		}
		public static float CollumnHeight {
			get { return EditorPrefs.GetFloat ("localization_wizard_collumn_height", 30f); }
			set { EditorPrefs.SetFloat ("localization_wizard_collumn_height", value); }
		}

		public static GUILayoutOption[] CollumnOptions() {
			return new GUILayoutOption[] {
				GUILayout.Width(CollumnWidth), GUILayout.Height(CollumnHeight)
			};
		}

		protected TextModalWindow RenameWindow { get { return (TextModalWindow)EditorWindow.GetWindow (typeof(TextModalWindow), true, "Rename"); } }

		[MenuItem ("LocalizationWizard/Wizard")]
		public static void CreateWindow() {
			CreateWindow (null);
		}

		public static void CreateWindow(ILocalizationManager manager){
			LocalizatonWizardEditorWindow window = (LocalizatonWizardEditorWindow)EditorWindow.GetWindow (typeof (LocalizatonWizardEditorWindow), false, "Wizard");
			window.Show();
			LocalizatonWizardEditorWindow.manager = manager;
		}

		void OnEnable() {
			settings = LocalizationSettings.GetSettings ();
			manager = null;
			defColor = GUI.color;
			defContentColor = GUI.contentColor;
			defBackgroundColor = GUI.backgroundColor;

			minSize = new Vector2 (700f, 400f);
		}

		void OnGUI() {
			EditorGUILayout.BeginVertical (GUI.skin.box);
			EditorGUILayout.Space ();
			EditorGUILayout.BeginHorizontal ();
			ILocalizationManager tempManager = manager;
			manager = EditorGUILayout.ObjectField ("Manager", manager, typeof(ILocalizationManager), true, GUILayout.Width(250f)) as ILocalizationManager;
			if (GUILayout.Button ("Scene", GUILayout.Width(80f)))
				manager = FindObjectOfType<ILocalizationManager> ();
			if (tempManager != manager && manager != null) {
				xmlData = new LocalizedXmlData ();
				xmlData.Read (settings.Languages);
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Space ();
			GUILayout.Label ("Language settings", EditorStyles.whiteLargeLabel);
			if (GUILayout.Button ("Open settings", GUILayout.Width (120f))) {
				EditorApplication.delayCall += () => {
					LocalizationSettings.ShowOrCreateSettingsData ();
				};
			}
			EditorGUILayout.Space ();
			GUILayout.Label ("GUI Settings", EditorStyles.whiteLargeLabel);
			CollumnWidth = DrawSlider ("Width", (int)CollumnWidth, "px", 40);
			CollumnHeight = DrawSlider ("Height", (int)CollumnHeight, "px", 20, 100);
			EditorGUILayout.Space ();
			EditorGUILayout.EndVertical ();

			if (manager) {
				EditorGUILayout.BeginVertical (GUI.skin.box);

				GUILayout.Label ("Values", EditorStyles.whiteLargeLabel);

				bool synchronizationNeeded = xmlData.documents.Count == 0 || xmlData.documents.Count != settings.LanguageStates.Count;

				if (!synchronizationNeeded) {
					foreach (SystemLanguage l in xmlData.documents.Keys)
						if (!settings.Languages.Contains (l)) {
							synchronizationNeeded = true;
							break;
						}
					foreach (SystemLanguage l in settings.Languages)
						if (!xmlData.documents.ContainsKey (l)) {
							synchronizationNeeded = true;
							break;
						}
				}

				if (synchronizationNeeded)
					EditorGUILayout.HelpBox ("Synchronization needed!", MessageType.Warning);
				
				if (synchronizationNeeded) {
					GUI.color = Color.cyan;
					if (GUILayout.Button ("Synchronize", GUILayout.Width (100f))) {
						SynchronizeData ();
					}
					GUI.color = defColor;
				} else {
					//	Filtering and saving group
					EditorGUILayout.BeginHorizontal ();
					GUI.color = changed ? Color.green : Color.grey;
					if (GUILayout.Button ("Save", GUILayout.Width (80f))) {
						changed = false;
						xmlData.SaveAll ();
					}
					GUI.color = defColor;

					GUILayout.Space (30f);
					GUILayout.Label ("Filter values", GUILayout.Width (80f));
					valuesFilter = EditorGUILayout.TextField (valuesFilter, GUILayout.Width(120f));
					GUI.color = Color.red;
					if (GUILayout.Button ("X", GUILayout.Width (17f), GUILayout.Height (17f))) {
						valuesFilter = string.Empty;
						GUI.FocusControl (null);
					}
					GUI.color = defColor;
					EditorGUILayout.EndHorizontal ();

					GUILayout.Space (10f);
                    
					scrollPos = EditorGUILayout.BeginScrollView (scrollPos);

                    //	Table headers
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("ID", GUILayout.Width(CollumnWidth));

                    foreach (SystemLanguage lang in xmlData.documents.Keys)
                    {
                        GUI.color = lang == settings.defaultLanguage ? Color.green : defColor;
                        if (GUILayout.Button(string.Format(lang == settings.defaultLanguage ? "{0} (default)" : "{0}", lang.ToString()), EditorStyles.label, GUILayout.Width(CollumnWidth)))
                        {
                            TextAsset asset = Resources.Load<TextAsset>(manager.GetFileName(lang));
                            if (asset)
                                EditorGUIUtility.PingObject(asset);
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    //	Draw table
                    DrawList();

					if (GUILayout.Button ("+", GUILayout.Width (25f), GUILayout.Height (25f))) {
						xmlData.AddKey (string.Empty);
						changed = true;
					}
					EditorGUILayout.EndScrollView ();
				}

				EditorGUILayout.EndVertical ();
			}
		}

		void DrawList() {
			string keyToRemove = null;
			int drawedCount = 0;
			foreach (string id in xmlData.data.Keys) {
				if (!string.IsNullOrEmpty (valuesFilter) && !id.Contains (valuesFilter))
					continue;
				else
					drawedCount++;
				
				EditorGUILayout.BeginHorizontal ();
				if (GUILayout.Button (id, EditorStyles.textField, CollumnOptions ())) {
					string finalId = id.Clone () as string;
					RenameWindow.Show (finalId, (string result) => {
						if (finalId.Equals(result))
							return;
						if (xmlData.data.ContainsKey (result))
							EditorUtility.DisplayDialog ("Duplicate id", string.Format ("ID [{0}] is already exists", result), "OK");
						else {
							xmlData.data = RenameKey<string, Dictionary<SystemLanguage, string>> (xmlData.data, finalId, result);
							changed = true;
						}
					});
				}

				foreach (SystemLanguage lang in xmlData.documents.Keys) {
					string temp = xmlData.data [id] [lang];
					xmlData.data [id] [lang] = DrawItem (xmlData.data [id] [lang]);
					if (!temp.Equals (xmlData.data [id] [lang]))
						changed = true;
				}

				GUI.backgroundColor = Color.red;
				GUI.contentColor = Color.white;
				if (GUILayout.Button ("X", GUILayout.Width (CollumnHeight), GUILayout.Height (CollumnHeight))) {
					keyToRemove = id;
				}
				GUI.backgroundColor = defBackgroundColor;
				GUI.contentColor = defContentColor;

				EditorGUILayout.EndHorizontal ();
			}
			if (drawedCount == 0)
				EditorGUILayout.HelpBox ("Nothing to show", MessageType.None);

			if (keyToRemove != null && xmlData.data.ContainsKey (keyToRemove)) {
				xmlData.data.Remove (keyToRemove);
				changed = true;
			}
		}

		protected string DrawItem(string item) {
			return manager.DrawRawItemInEditor (item, CollumnWidth, CollumnHeight);
		}

		void SynchronizeData() {
			manager.SynchronizeData ();
			xmlData = new LocalizedXmlData ();
			xmlData.Read (settings.Languages);
			DeleteUnusedLangFiles ();
			changed = true;
		}

		public class LocalizedXmlData {
			public Dictionary<SystemLanguage, XmlDocument> documents = new Dictionary<SystemLanguage, XmlDocument>();
			public Dictionary<string, Dictionary<SystemLanguage, string>> data = new Dictionary<string, Dictionary<SystemLanguage, string>>();

			public void Read(List<SystemLanguage> languages) {
				documents.Clear ();
				data.Clear ();
				foreach (SystemLanguage l in languages) {
					XmlDocument doc = LocalizationXmlUtil.LoadFile (manager.GetFileName (l));
					if (doc != null)
						documents.Add (l, doc);
				}
				
				string[] keys = LocalizationXmlUtil.GetKeys (documents.Values);
				foreach (string key in keys) {
					Dictionary<SystemLanguage, string> element = new Dictionary<SystemLanguage, string> ();
					foreach (SystemLanguage lang in languages) {
						XmlDocument doc = documents [lang];
						if (LocalizationXmlUtil.ContainsValue (doc, key))
							element.Add (lang, LocalizationXmlUtil.GetValue (doc, key));
						else
							element.Add (lang, string.Empty);
					}
					data.Add (key, element);
				}
			}

			public void Save(SystemLanguage language) {
				XmlDocument doc = LocalizationXmlUtil.EmptyXml;
				foreach (string id in data.Keys) {
					LocalizationXmlUtil.WriteValue (doc, id, data [id] [language]);
				}
				LocalizationXmlUtil.SaveFile (doc, LocalizationWizardUtil.GetPath(manager.GetFileName (language), LocalizationWizardUtil.TYPE_XML), LocalizationXmlUtil.FileMode.REWRITE);
			}

			public void SaveAll() {
				foreach (SystemLanguage lang in documents.Keys)
					Save (lang);
			}

			public void AddKey(string key) {
				if (data.ContainsKey (key))
					return;
				data.Add (key, new Dictionary<SystemLanguage, string> ());
				foreach (SystemLanguage lang in documents.Keys)
					data [key].Add (lang, string.Empty);
			}
		}

		Dictionary<T, K> RenameKey<T, K>(Dictionary<T, K> dict, T fromKey, T toKey) {
			if (dict.ContainsKey (fromKey)) {
				K temp = dict [fromKey];
				dict.Remove (fromKey);
				dict [toKey] = temp;
			}
			return dict;
		}

		void DeleteUnusedLangFiles() {
			List<SystemLanguage> languages = settings.Languages;
			foreach (SystemLanguage lang in Enum.GetValues(typeof(SystemLanguage))) {
				XmlDocument doc = LocalizationXmlUtil.LoadFile (manager.GetFileName (lang));
				if (doc != null && !languages.Contains (lang))
					LocalizationXmlUtil.DeleteFile (LocalizationWizardUtil.GetPath (manager.GetFileName (lang), LocalizationWizardUtil.TYPE_XML));
			}
		}

		int DrawSlider(string label, int value, string postFix, int min = 20, int max = 300, float labelWidth = 70f, float sliderWidth = 150f) {
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label (label, GUILayout.Width (labelWidth));
			value = EditorGUILayout.IntSlider (value, min, max, GUILayout.Width(sliderWidth));
			GUILayout.Label (postFix);
			EditorGUILayout.EndHorizontal ();
			return value;
		}
	}
}

#endif
