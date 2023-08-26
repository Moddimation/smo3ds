using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_persistent : MonoBehaviour {
	
	public bool isPersistent = true;

	// Use this for initialization
	void Start () {
		if(isPersistent) DontDestroyOnLoad (gameObject);
	}
}
