﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class scr_title : MonoBehaviour {

	private Animator anim;
	private Vector3 marioPos;

	public GameObject spr_rotMap;
	public GameObject spr_shade;
	public Transform cnv_down;
	Material mat_rotMap;
	Material mat_shade;
	bool bvar0 = false;

	public float speed;
	public Color startColor, endColor;
	public Color startColorShade, endColorShade;
	public GameObject buttonRes;//resume button

	public static scr_title _f;


	public IEnumerator ChangeEngineColour()
	{
		float tick = 0f;
		while (mat_rotMap.color != endColor)
		{
			tick += Time.unscaledDeltaTime * speed;
			mat_rotMap.color = Color.Lerp(startColor, endColor, tick);
			mat_shade.color = Color.Lerp(startColorShade, endColorShade, tick);
			yield return null;
		}
	}

	// Use this for initialization
	void Start(){
		anim = GetComponent<Animator> ();
		marioPos = transform.position; // to move him back later
		transform.position = new Vector3 (-1000, transform.position.y, transform.position.z); //move to waitzone, works like a timer.
		scr_fadefull._f.Run (true, 0, 0.02f);//fade in
		mat_rotMap = spr_rotMap.gameObject.GetComponent<MeshRenderer> ().material;
		mat_shade = spr_shade.gameObject.GetComponent<MeshRenderer> ().material;//get materials
		scr_main._f.focusOff ();
		_f = this;
	}

	// Update is called once per frame
	void Update () {
		if (scr_fadefull._f.isDone || bvar0) {
			if (!bvar0) {
				scr_manageAudio._f.AudioStart ("Sound/Entity/Mario/snd_MarioTitleName", false);
				bvar0 = true;
			} else {
				if (scr_manageAudio._f.isPlaying()) {
					if (transform.position.x < -1008f) {
						transform.position = marioPos;
						anim.Play ("titleStart");
					}
					transform.Translate (new Vector3 (0.1f, 0, 0));
				} else {
					scr_main._f.focusOn ();
					for (int i = 0; i < 4; i++)
						cnv_down.GetChild (i).gameObject.SetActive (true);
					EventSystem.current.SetSelectedGameObject (buttonRes);
					scr_manageAudio._f.AudioStart ("Music/Bgm/bgmTitle");
					this.enabled = false;
				}
			}
		}
	}
}