using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class anim_mapRotate : MonoBehaviour {
	
	public GameObject sprite_grey;//theres a grey one, made out of the red one, that one will be used later, as i can change the color more easy.
	public float rotationSpeed = 0.0001f;//yep
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		sprite_grey.transform.Rotate(0f, 0f, rotationSpeed);//and thats it, really.
	}
}
