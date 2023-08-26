using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_levelInOdyssey : MonoBehaviour {

	void Update () {
		if (MarioController.marioObject != null) {
			MarioCam.marioCamera.camYOffset = 3;
			MarioCam.marioCamera.setState (1, transform.GetChild (0).transform.position, transform.GetChild (0).transform.rotation);
			Destroy (transform.GetChild (0).gameObject); //destroy child
			Destroy(gameObject);
		}
	}
}
