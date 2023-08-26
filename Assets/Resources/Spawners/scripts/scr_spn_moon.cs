using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_spn_moon : MonoBehaviour {

	public string moonName = "Hidden from the Developers";
	// Use this for initialization
	void Update () {
		scr_summon.f_summon.s_object(6, transform.position, transform.eulerAngles).gameObject.GetComponent<scr_behaviorMoon>().moonName=moonName;
		Destroy(gameObject);
	}
}
