﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scr_behaviorTransCap : MonoBehaviour {

	bool isFlyingIn = false;
	SpriteRenderer sprRender;
	// Use this for initialization
	void OnEnable () {
		if (!isFlyingIn) {
			sprRender.color = new Color (0,0,0,0);
			scr_main.s.SetFocus(false);
			transform.localScale = new Vector3 (20, 20*0.95f, 0);
		} else {
			transform.localScale = new Vector3 (0, 0, 0);
		}
	}
	void Awake(){
		sprRender = gameObject.GetComponent<SpriteRenderer>();
		gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if(!isFlyingIn){
			transform.localScale -= new Vector3(1f, 1f, 0);
			sprRender.color = new Color (0,0,0,gameObject.GetComponent<SpriteRenderer>().color.a + 0.1f);
			if(transform.localScale.x <0.2f){
				isFlyingIn = true;
				SceneManager.LoadScene (scr_loadScene.s.nextScene);
				scr_main.s.SetFocus(true);
			}
		} else if(scr_main.s.hasLevelLoaded){
			transform.localScale += new Vector3(3f, 3f, 0);
			if(transform.localScale.x > 20){
				isFlyingIn = false;
				gameObject.SetActive (false);
			}
		}
	}
}
