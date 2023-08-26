using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LocalizationWizard {
	public abstract class ILocalizableEntity : MonoBehaviour {
		public ILocalizationManager manager;
		public string ID;

		public virtual void OnEnable() {
			if (manager == null) {
				Debug.LogErrorFormat ("LocalizableEntity - {0} : LocalizationManager is not assigned!", gameObject.name); 
				return;
			}

			if (manager.IsAvailable ())
				Apply ();

			manager.onLanguageUpdated.AddListener (LocalizationManager_onLanguageChanged);
		}

		public virtual void OnDisable() {
			manager.onLanguageUpdated.RemoveListener (LocalizationManager_onLanguageChanged);
		}

		void LocalizationManager_onLanguageChanged (SystemLanguage newLanguage)
		{
			Apply ();
		}

		public bool FindCompatibleManager() {
			if (manager == null) {
				foreach (ILocalizationManager m in FindObjectsOfType<ILocalizationManager>())
					if (IsManagerCompatible(m)) {
						manager = m;
						break;
					}
			}
			return manager != null;
		}

		public void Apply() {
			Apply (ID);
		}

		public abstract bool IsManagerCompatible (ILocalizationManager manager);
		public abstract void Apply(string id);
	}

	#if UNITY_EDITOR
	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(ILocalizableEntity))]
	public class ILocalizableEntityEditor : UnityEditor.Editor {
		const float ID_BUTTON_HEIGHT = 15f;
		const string ID_UNDEFINED = "- none -";

		protected ILocalizableEntity entity;
		protected UnityEditor.SerializedProperty idProp, managerProp;

		protected List<string> identifyers = new List<string>();
		protected bool isChangingId = false;

		Vector2 scrollPosition;
		string filterString = string.Empty;
		Color defColor, defBackgroundColor;

		protected virtual void OnEnable() {
			entity = target as ILocalizableEntity; 
			managerProp = serializedObject.FindProperty("manager");
			idProp = serializedObject.FindProperty ("ID");

			if (entity.manager == null)
				entity.FindCompatibleManager ();

			isChangingId = false;
			filterString = string.Empty;

			defColor = GUI.color;
			defBackgroundColor = GUI.backgroundColor;
		}

		protected virtual void OnDisable() {
			isChangingId = false;
			filterString = string.Empty;
		}

		public sealed override void OnInspectorGUI ()
		{
			serializedObject.Update ();
			UnityEditor.EditorGUI.BeginChangeCheck();

			UnityEditor.EditorGUILayout.PropertyField (managerProp, new GUIContent("Manager"));
			GUILayout.Space (10f);

			if (entity.manager) {
				UpdateIdentifyers ();

				if (entity.IsManagerCompatible(entity.manager)) {
					DrawDefaultGUI ();
					DrawExtensionsGUI ();
				} else
					UnityEditor.EditorGUILayout.HelpBox (string.Format ("{0} not compatible with current LocalizableEntity!\nGameObject not contains needed Component.", entity.manager.GetType().Name), UnityEditor.MessageType.Error);
			} else {
				UnityEditor.EditorGUILayout.HelpBox ("Select / Find LocalizationManager", UnityEditor.MessageType.Warning);
				if (GUILayout.Button ("Find"))
					entity.FindCompatibleManager ();
			}

			UnityEditor.EditorGUI.EndChangeCheck();
			serializedObject.ApplyModifiedProperties ();
		}

		protected virtual void DrawDefaultGUI() {
			if (identifyers.Count == 0) {
				UnityEditor.EditorGUILayout.HelpBox ("Localization data is empty", UnityEditor.MessageType.Info);
			} else {
				GUI.color = isChangingId ? Color.grey : defColor;

				UnityEditor.EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("ID");
				if (GUILayout.Button (idProp.hasMultipleDifferentValues ? "-" : entity.ID, UnityEditor.EditorStyles.popup))
					isChangingId = !isChangingId;
				UnityEditor.EditorGUILayout.EndHorizontal ();

				GUI.color = defColor;

				if (isChangingId)
					DrawIdentifyers ();
			}
		}

		protected virtual void DrawExtensionsGUI() {
			
		}

		void UpdateIdentifyers() {
			entity.manager.LoadData ();
			identifyers.Clear ();
			identifyers.Add (ID_UNDEFINED);
			identifyers.AddRange (LocalizationXmlUtil.GetKeys (entity.manager.Data));
			identifyers.Sort ();
		}

		void DrawIdentifyers() {
			GUILayout.Space (10f);
			UnityEditor.EditorGUILayout.BeginVertical (GUI.skin.box);

			UnityEditor.EditorGUILayout.BeginHorizontal ();
			filterString = UnityEditor.EditorGUILayout.TextField ("Filter", filterString);
			if (GUILayout.Button ("cancel", GUILayout.Width (50f))) {
				filterString = string.Empty;
				isChangingId = false;
				GUI.FocusControl (null);
			}
			UnityEditor.EditorGUILayout.EndHorizontal ();

			scrollPosition = UnityEditor.EditorGUILayout.BeginScrollView (scrollPosition, GUI.skin.box, GUILayout.Height (ID_BUTTON_HEIGHT * Mathf.Clamp (identifyers.Count, 3, 6)));

			foreach (string id in identifyers) {
				if ((id.Contains(filterString) || filterString.Length == 0) && GUILayout.Button (id, UnityEditor.EditorStyles.label, GUILayout.Height(ID_BUTTON_HEIGHT))) {
					foreach (object t in targets) {
						if (t.GetType () == target.GetType ()) {
							(t as ILocalizableEntity).ID = id;
							if (!ID_UNDEFINED.Equals(id))
								(t as ILocalizableEntity).Apply ();
						}
					}

					isChangingId = false;
					filterString = string.Empty;
				}
			}
			UnityEditor.EditorGUILayout.EndVertical ();
			UnityEditor.EditorGUILayout.EndScrollView ();
		}
	}
	#endif
}
