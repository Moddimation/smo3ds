using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scr_loadScene : MonoBehaviour {
	
	[HideInInspector] public static scr_loadScene s;
	[HideInInspector] public string nextScene = "scn_menuTitle";
	[HideInInspector] public bool isDone = false;
	AsyncOperation loadOP;

    //List<GameObject> rootObjects = new List<GameObject>();

    void Start(){ s = this; }
	public void StartScene(string sceneName, int transition = 0){
		isDone = false;

		//GetComponent<AudioListener>().enabled = true;
		scr_main.s.hasLevelLoaded = false;
		nextScene = sceneName;
		scr_main.s.dbg_enemyCount = 0;

		scr_main.DPrint ("nSCN: " + nextScene);
		switch (transition) {
		case 0: //direct, no transition
			SceneManager.LoadScene (sceneName);
			break;
		case 1: //flying ship line
			scr_main.s.SetFocus(false);
			SceneManager.LoadScene ("scn_loadShip");
			nextScene = sceneName;
				//wip
			break;
		case 2://cap fly transition
			scr_main.s.transform.GetChild (1).GetChild (2).gameObject.SetActive (true);
            break;
		case 3: //async  (need to unload old one manually when needed)
            StartCoroutine (loadAsync ());
			break;
		}
	}
	IEnumerator loadAsync(){
		loadOP = SceneManager.LoadSceneAsync (nextScene, LoadSceneMode.Additive);
		loadOP.allowSceneActivation = false;
		while (!loadOP.isDone) {
			scr_main.DPrint ("loading: " + (loadOP.progress*100) + "%", false);
			yield return null;
		}
		scr_main.DPrint ("loading: 100%");
		isDone = true;
		Scene loaded = SceneManager.GetSceneByName(nextScene);
		if (loaded.IsValid()) {
			SceneManager.SetActiveScene(loaded);
		}
	}
	public void SetSceneActive(){
		loadOP.allowSceneActivation = true;
	}
}
