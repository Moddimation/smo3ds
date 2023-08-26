using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_behaviorCoin : MonoBehaviour {

	public int currentState = 0;
	private int statesState = 0;
	private int myPassedFrame = 0;
	AudioSource sndSrc;
	// Use this for initialization
	void Start () {
		sndSrc = GetComponent<AudioSource> ();
	}
	void OnTouch(int numType){
		if(currentState==0) if(numType == 1 || numType == 2){
			scr_gameInit.globalValues.coinsCount++;//add coin to global
			sndSrc.Play();
			transform.GetChild (1).GetComponent<SkinnedMeshRenderer> ().enabled = false;
			currentState = 2;
			scr_manageEffect._f.Play("prt_coinSpark0", transform.position, transform.rotation, "prt_coinSpark1");
		}
	}
	// Update is called once per frame
	void Update () {
		switch(currentState){
			case 0://static rotation, fly
				transform.Rotate(0, 10f, 0);
				break;
			case 1://jump out of *, collected
				switch(statesState){
					case 0:
						transform.Rotate(0, 14f, 0);
						transform.Translate(0,0.4f,0);
						myPassedFrame++;
						if(myPassedFrame>6){
							statesState++;
							myPassedFrame=0;
						}
						break;
					case 1:
						myPassedFrame++;
						transform.Rotate(0, 14f, 0);
						if(myPassedFrame>8){
							scr_gameInit.globalValues.coinsCount++;
							Destroy(gameObject);
						}
						break;
				}
				break;
		case 2: //dying.
			if (!sndSrc.isPlaying)
				Destroy (gameObject);
			break;
		}
	}
}
