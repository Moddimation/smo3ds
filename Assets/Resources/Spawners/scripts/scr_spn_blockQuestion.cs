using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_spn_blockQuestion : MonoBehaviour {

	public int myType = 0; //0= single coin, 1 = 10 coints, etc
	// Use this for initialization
	void Awake () {
		scr_summon.f_summon.s_object(7, transform.position, transform.eulerAngles).GetComponent<scr_behaviorBlockQuestion>().myType = myType;
		Destroy(gameObject);
	}
}
