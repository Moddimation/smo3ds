using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_behaviorCapDoor : MonoBehaviour {

	public int currentState = 0;
	private Animator anim;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
	}
	void OnTriggerEnter(Collider collis){
		if (collis.gameObject.name == "objMarioCap"&&MarioController.marioObject.cappy.isThrown){
			anim.Play("open");
			transform.GetChild (5).gameObject.SetActive (false);
		}
	}
	// Update is called once per frame
}
