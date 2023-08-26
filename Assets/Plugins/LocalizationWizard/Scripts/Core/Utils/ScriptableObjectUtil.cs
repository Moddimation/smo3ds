using UnityEngine;
using System.Collections;

namespace LocalizationWizard {
	
	public sealed class ScriptableObjectUtil {

		public static T Get<T>(string fileName) where T : ScriptableObject {
			T data = null;

			#if UNITY_EDITOR
			data = UnityEditor.AssetDatabase.LoadAssetAtPath<T> (LocalizationWizardUtil.GetPath(fileName));
			if (!data) {
				data = ScriptableObject.CreateInstance<T>();

				UnityEditor.AssetDatabase.CreateAsset (data, LocalizationWizardUtil.GetPath (fileName));
				UnityEditor.AssetDatabase.SaveAssets ();
			}
			#else
			data = Resources.Load<T> (fileName);
			#endif
			return data;
		}
	}
}