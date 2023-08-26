using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace LocalizationWizard {
	public class LocalizationXmlUtil {
		public enum FileMode { CREATE, REWRITE }

		private const string BASE_ELEMENT_NAME = "data";
		private const string LANG_ELEMENT_NAME = "text";

		private const string FILENAME_PATH_FORMAT = LocalizationWizardUtil.RESOURCES_FOLDER_PATH + "/{0}.xml";
		private const string NODE_FORMAT = BASE_ELEMENT_NAME + "/" + LANG_ELEMENT_NAME + "[@id = '{0}']";

		public static XmlDocument LoadFile(string fileName) {
			TextAsset langFile = Resources.Load<TextAsset>(fileName);
			if (!langFile)
				return null;
			XmlDocument doc = new XmlDocument ();
			doc.LoadXml (langFile.text);
			Resources.UnloadAsset(langFile);
			return doc;
		}

		public static bool DeleteFile(string path) {
			#if UNITY_EDITOR
			bool exists = File.Exists(path);
			if (exists) {
				UnityEditor.AssetDatabase.DeleteAsset(path);
				UnityEditor.AssetDatabase.Refresh();
			}
			return exists;
			#else
			return true;
			#endif
		}

		public static XmlDocument SaveFile(XmlDocument doc, string path, LocalizationXmlUtil.FileMode fileMode) {
			#if UNITY_EDITOR
			string[] pathSplitted = path.Split(new char[] { '/' });
			string fileDir = string.Empty;
			for (int i = 0; i < pathSplitted.Length - 1; i++)
				fileDir += pathSplitted[i] + "/";

			if (!Directory.Exists(fileDir))
				Directory.CreateDirectory(fileDir);
			
			if (fileMode == FileMode.REWRITE && File.Exists(path))
				UnityEditor.AssetDatabase.DeleteAsset(path);
			if (!File.Exists(path) || fileMode != FileMode.CREATE)
				doc.Save(path);
			
			UnityEditor.AssetDatabase.Refresh();
			#else
			Debug.Log("Saving lang data available only in UnityEditor");
			#endif
			return doc;
		}

		public static string GetValue(XmlDocument document, string id) {
			XmlNode value = document.SelectSingleNode (string.Format (NODE_FORMAT, id));
			if (value != null)
				return value.InnerText;
#if UNITY_5_3_OR_NEWER
			Debug.LogAssertionFormat ("Cannot resolve text ID: {0}", id);
#else
			Debug.LogWarningFormat ("Cannot resolve text ID: {0}", id);
#endif
			return null;
		}

		public static void WriteValue(XmlDocument doc, string id, string value) {
			XmlElement baseNode = (XmlElement)doc.GetElementsByTagName (BASE_ELEMENT_NAME)[0];
			XmlNode newNode = null;
			if (ContainsValue (doc, id)) {
				newNode = doc.SelectSingleNode (string.Format (NODE_FORMAT, id));
				newNode.Attributes.RemoveAll ();
				newNode.RemoveAll ();
			} else
				newNode = doc.CreateNode (XmlNodeType.Element, LANG_ELEMENT_NAME, null);
			XmlAttribute idAttr = doc.CreateAttribute ("id");
			idAttr.Value = id;
			newNode.Attributes.Append (idAttr);
			newNode.AppendChild (doc.CreateTextNode (value));

			if (baseNode.ChildNodes.Count == 0)
				baseNode.AppendChild (newNode);
			else
				baseNode.PrependChild (newNode);
		}

		public static bool DeleteValue(XmlDocument doc, string id) {
			bool result = ContainsValue (doc, id);

			if (result) {
				XmlNode node = doc.SelectSingleNode (string.Format (NODE_FORMAT, id));
				if (node.ParentNode != null)
					node.ParentNode.RemoveChild (node);
				else
					doc.RemoveChild (node);
			}
			return result;
		}

		public static bool ContainsValue(XmlDocument doc, string id) {
			return doc.SelectSingleNode (string.Format (NODE_FORMAT, id)) != null;
		}

		public static string[] GetValues(XmlDocument doc) {
			List<string> values = new List<string> ();
			XmlElement baseNode = (XmlElement)doc.GetElementsByTagName (BASE_ELEMENT_NAME) [0];
			Debug.Log (doc.GetElementsByTagName (BASE_ELEMENT_NAME));
			foreach (XmlNode node in baseNode.ChildNodes)
				if (node.Attributes ["id"] != null)
					values.Add (node.Attributes ["id"].InnerText);
			return values.ToArray();
		}

		public static string[] GetKeys(XmlDocument doc) {
			List<string> result = new List<string> ();

			if (doc != null) {
				XmlElement baseNode = (XmlElement)doc.GetElementsByTagName (BASE_ELEMENT_NAME)[0];
				foreach (XmlNode n in baseNode.ChildNodes) {
					if (n.Attributes.Count > 0) {
						result.Add (n.Attributes.Item (0).InnerText);
					}
				}
			}
			return result.ToArray();
		}

		public static string[] GetKeys(IEnumerable<XmlDocument> docs) {
			List<string> result = new List<string> ();
			foreach (XmlDocument doc in docs) {
				foreach (string key in GetKeys(doc))
					if (!result.Contains (key))
						result.Add (key);
			}
			return result.ToArray();
		}

		public static string EmptyXmlRaw {
			get {
				System.Text.StringBuilder sb = new System.Text.StringBuilder ();

				XmlWriter writer = XmlWriter.Create(sb);
				writer.WriteStartDocument();
				writer.WriteStartElement (BASE_ELEMENT_NAME);
				writer.WriteEndElement ();
				writer.WriteEndDocument();

				writer.Flush();
				return sb.ToString ();
			}
		}

		public static XmlDocument EmptyXml {
			get {
				XmlDocument doc = new XmlDocument ();
				doc.LoadXml (EmptyXmlRaw);
				return doc;
			}
		}
	}
}
