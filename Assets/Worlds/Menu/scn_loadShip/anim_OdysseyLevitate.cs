using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class anim_OdysseyLevitate : MonoBehaviour {

	public float lev_offset = 0.0001f; //offset to move
	public float lev_max = 1f; //maximal levitation offset
	private float lev_height;

	private float lev_count = 0f; // stores the changing levitation value
	private bool lev_up = true;
	public GameObject loadingLine;
	
	// Update is called once per frame
	void Start () {
		lev_height = transform.position.x;
	}
	void Update () {
		transform.position = new Vector3(lev_height-lev_count, transform.position.y-6f, transform.position.z);
		loadingLine.transform.position = new Vector3(loadingLine.transform.position.x, loadingLine.transform.position.y-6f, loadingLine.transform.position.z);
		if(!lev_up){
		    lev_count+=lev_offset;
		    if(lev_count>lev_max) lev_up = true;
		} else {
		    lev_count-=lev_offset;
		    if(lev_count< -lev_max) lev_up = false;
		}
		if(transform.position.y<-50 && transform.position.y>-56){
			SceneManager.LoadScene (scr_loadScene._f.nextScene, LoadSceneMode.Additive);
			scr_gameInit.globalValues.focusOn();
		}
		if(transform.position.y<-284) Destroy (GameObject.FindWithTag("loading"));
	}
}
