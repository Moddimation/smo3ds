using UnityEngine;
using System.Collections;

namespace LocalizationWizard {
	public abstract class LocalizationManager<Item> : ILocalizationManager {
		#if UNITY_EDITOR
		public abstract Item DrawItemInInspector(Item item, float width, float height);
		#endif

		protected abstract Item ResolveRaw(string raw);
		protected abstract string WriteRaw (Item value);

		public Item GetValue(string id) {
			return ResolveRaw (GetRawValue (id));
		}

		public void AddValue(string id, Item item) {
			AddRawValue (id, WriteRaw (item));
		}

		#if UNITY_EDITOR
		public sealed override string DrawRawItemInEditor (string item, float width, float height)
		{
			return WriteRaw (DrawItemInInspector (ResolveRaw (item), width, height));
		}
		#endif

		#if UNITY_EDITOR
		protected static ManagerType InstantiateManager<ManagerType>(string fileName = null) where ManagerType : ILocalizationManager {
			GameObject baseGO = GameObject.Find ("LocalizationWizard");
			if (!baseGO)
				baseGO = new GameObject ("LocalizationWizard");

			GameObject managerGO = new GameObject (typeof(ManagerType).Name);
			ManagerType manager = managerGO.AddComponent<ManagerType> ();
			if (!string.IsNullOrEmpty (fileName))
				manager.fileName = fileName;
			managerGO.transform.SetParent (baseGO.transform);
			return manager;
		}
		#endif
	}
}
