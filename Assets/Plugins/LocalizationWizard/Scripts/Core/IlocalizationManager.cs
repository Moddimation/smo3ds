using UnityEngine;
using System.Collections;
using System.Xml;
using UnityEngine.Events;

namespace LocalizationWizard {
	[System.Serializable]
	public class SystemLanguageUnityEvent : UnityEvent<SystemLanguage> {}

	public abstract class ILocalizationManager : MonoBehaviour {
		[Header("File")]
		public string fileName = "Data";
		[Space]
		public SystemLanguageUnityEvent onLanguageUpdated;

		protected static LocalizationSettings Settings { get { return LocalizationSettings.GetSettings (); } }
		public XmlDocument Data { get; protected set; }

		#if UNITY_EDITOR
		public abstract string DrawRawItemInEditor(string item, float width, float height);
		#endif

		public virtual string GetFileName(SystemLanguage language){
			return string.Format ("{0}/{1}", language, fileName);
		}

		public XmlDocument SynchronizeData(){
			foreach (SystemLanguage language in Settings.Languages)
				LocalizationXmlUtil.SaveFile (LocalizationXmlUtil.EmptyXml, LocalizationWizardUtil.GetPath (GetFileName (language), LocalizationWizardUtil.TYPE_XML), LocalizationXmlUtil.FileMode.CREATE);
			return LoadData () ? Data : null;
		}

		public bool LoadData() {
			Data = LocalizationXmlUtil.LoadFile (GetFileName (LocalizationSettings.UserLanguage));
			return Data != null;
		}

		protected bool CheckDataValidity() {
			if (Data == null)
				throw new System.NullReferenceException ("LocalizationData has not been initialized! Call Create method first.");
			else
				return true;
		}

		protected virtual void Awake(){
			LoadData ();
		}

		public bool IsAvailable() {
			return Data != null;
		}

		protected virtual void OnEnable() {
			onLanguageUpdated.Invoke (LocalizationSettings.UserLanguage);
			LocalizationManager.onLanguageChanged += OnLanguageChanged;
		}

		protected virtual void OnDisable() {
			LocalizationManager.onLanguageChanged -= OnLanguageChanged;
		}

		protected string GetRawValue(string id) {
			CheckDataValidity ();

			return LocalizationXmlUtil.GetValue (Data, id);
		}

		protected void AddRawValue(string id, string item) {
			CheckDataValidity ();

			LocalizationXmlUtil.WriteValue (Data, id, item);
		}

		public bool RemoveValue(string id) {
			CheckDataValidity ();

			return LocalizationXmlUtil.DeleteValue (Data, id);
		}

		public void Save() {
			CheckDataValidity ();

			LocalizationXmlUtil.SaveFile (Data, LocalizationWizardUtil.GetPath (fileName, LocalizationWizardUtil.TYPE_XML), LocalizationXmlUtil.FileMode.REWRITE);
		}

		void OnLanguageChanged (SystemLanguage newLanguage)
		{
			LoadData ();
			onLanguageUpdated.Invoke (newLanguage);
		}
	}

	public sealed class LocalizationManager {
		public delegate void LanguageChangeDelegate(SystemLanguage newLanguage);

		public static event LanguageChangeDelegate onLanguageChanged;

		public static void OnLanguageChanged(SystemLanguage language){
			if (onLanguageChanged != null)
				onLanguageChanged.Invoke (language);
		}
	}
}
