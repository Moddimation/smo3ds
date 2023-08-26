using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;

public abstract class ModalWindow<T> : EditorWindow {
	public delegate void ResultDelegate(T result);
	public delegate void CancelDelegate ();

	private ResultDelegate success;
	private CancelDelegate cancel;
	protected T input { get; set; }

	public void Show(T input, ResultDelegate success, CancelDelegate cancel = null) {
		this.input = input;
		this.success = success;
		this.cancel = cancel;

		maxSize = new Vector2 (200f, 60f);
		minSize = maxSize;

		ShowPopup ();
	}

	public void SendResult(T result) {
		if (success != null)
			success.Invoke (result);
		this.Close ();
	}

	protected virtual void OnLostFocus() {
		if (cancel != null)
			cancel.Invoke ();
		this.Close ();
	}

	void OnGUI() {
		DrawGUI ();
	}

	protected abstract void DrawGUI ();

}
#endif
