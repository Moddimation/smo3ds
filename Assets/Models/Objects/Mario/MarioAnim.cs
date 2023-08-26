using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioAnim : MonoBehaviour {

	Animator anim;
	public MarioAnim _a;

	void Start () {
		anim = GetComponent<Animator> ();
	}
	void Update () {
		_a = this;
	}
}
