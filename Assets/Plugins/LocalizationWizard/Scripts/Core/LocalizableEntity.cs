using UnityEngine;
using System.Collections;

namespace LocalizationWizard {

	public abstract class LocalizableEntity<Item, LocalizationManagerType> : ILocalizableEntity where LocalizationManagerType : LocalizationManager<Item> {

		public abstract void ApplyValue (Item value);

		protected Item GetItem(string id) {
			if (IsManagerCompatible (manager) && manager.IsAvailable())
				return (manager as LocalizationManagerType).GetValue (id);
			else
				return default(Item);
		}

		public override bool IsManagerCompatible (ILocalizationManager manager)
		{
			return manager is LocalizationManagerType;
		}

		public sealed override void Apply (string id)
		{
			if (FindCompatibleManager () && manager.IsAvailable ())
				ApplyValue (GetItem (id));
			else
				Debug.LogErrorFormat ("LocalizableEntity - {0} : LocalizationManager is not assigned or not available.", gameObject.name); 
		}
	}
}
