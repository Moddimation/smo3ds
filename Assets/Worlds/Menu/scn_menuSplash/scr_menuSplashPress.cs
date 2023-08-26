using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class scr_menuSplashPress : MonoBehaviour {

	void Confirm(){
		Debug.Log ("Loading...");
		//scr_loadScene._f.loadTransition ("scn_capMain0", 1);
		UnityEngine.SceneManagement.SceneManager.LoadScene("scn_menuTitle");
		scr_manageData._f.Load ();
	}

	void Start(){
		scr_fadefull._f.Run (true, 0, 0.02f);
	}

	void Update () {
		#if UNITY_EDITOR
		if(Input.GetKey(KeyCode.Return)) Confirm();
		#else
		if(UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.A)){ 
			if(scr_gameInit.globalValues != null){
				if(scr_gameInit.globalValues.isFocused) Confirm();
			} else Confirm();
		}
		#endif
	}
}