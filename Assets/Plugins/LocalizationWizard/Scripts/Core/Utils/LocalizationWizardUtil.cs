using UnityEngine;
using System.Collections;

namespace LocalizationWizard {
	public struct LocalizationWizardUtil {
		public const string FOLDER_NAME = "LocalizationWizard";
		public const string RESOURCES_FOLDER_PATH = "Assets/" + FOLDER_NAME + "/Resources";

		public const string TYPE_ASSET = "asset";
		public const string TYPE_XML = "xml";

		public const string LANGUAGE_PREF_TAG = "application_current_language";

		public static string GetPath(string fileName) {
			return GetPath (fileName, TYPE_ASSET);
		}

		public static string GetPath(string fileName, string type) {
			return string.Format (LocalizationWizardUtil.RESOURCES_FOLDER_PATH + "/{0}.{1}", fileName, type);
		}

		public static string GetResourceName(string assetPath) {
			if (assetPath.Contains ("/Resources/")) {
				assetPath = assetPath.Substring (assetPath.IndexOf ("Resources/") + "Resources/".Length);
				assetPath = assetPath.Substring (0, assetPath.IndexOf ('.'));
				return assetPath;
			} else
				return string.Empty;
		}
	}
}
