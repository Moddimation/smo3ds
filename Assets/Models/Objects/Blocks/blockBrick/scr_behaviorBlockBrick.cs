using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_behaviorBlockBrick : MonoBehaviour {

	Animator anim;
	public int myType = 0;
	bool isHit = false;
	bool wasHit = false;
	int hitCount = 0;
	
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
	}
	
	void isEmpty(){
		scr_summon.f_summon.s_object(9, transform.position, transform.eulerAngles);
		Destroy(gameObject);
	}
	
	void myDestroy(){
		GameObject.Instantiate(Resources.Load<GameObject>("Objects/objBlockBrickBreak"), transform.position, transform.rotation);
		Destroy(gameObject);
	}
	
	public void OnTouch(int numType){
		switch (numType) {
		case 2:
			if(!wasHit) isHit = true;
			break;
		case 1:
			isHit = true;
			break;
		}
	}
	
	void Update(){
		if(wasHit) if(MarioController.marioObject.isGrounded) wasHit = false;
		if(isHit){
			isHit = false;
			wasHit = true;
			hitCount++;
			switch(myType){
				case 0: //empty
					myDestroy();
					break;
				case 1: //1 coin
					scr_summon.f_summon.s_object(0, new Vector3(transform.position.x, transform.position.y+0.8f, transform.position.z), transform.eulerAngles).GetComponent<scr_behaviorCoin>().currentState = 1;
					if(hitCount==1) isEmpty(); else anim.Play("up");
					break;
				case 2: //5 coin
					scr_summon.f_summon.s_object(0, new Vector3(transform.position.x, transform.position.y+0.8f, transform.position.z), transform.eulerAngles).GetComponent<scr_behaviorCoin>().currentState = 1;
					if(hitCount==5) isEmpty(); else anim.Play("up");
					break;
				case 3: //10 coin
					scr_summon.f_summon.s_object(0, new Vector3(transform.position.x, transform.position.y+0.8f, transform.position.z), transform.eulerAngles).GetComponent<scr_behaviorCoin>().currentState = 1;
					if(hitCount==10) isEmpty(); else anim.Play("up");
					break;
			}
		}
	}
}
