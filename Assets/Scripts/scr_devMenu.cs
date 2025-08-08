#if UNITY_EDITOR
#define isDebug
#endif
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class scr_devMenu: MonoBehaviour {
	#if true

	private bool isOpen = false;
	private float ypos = 14;
	private float xoffset = 10;
	private int selection = 4;
	private bool noButtonPressed = true;
	private bool submenu = false;
	private int selectionSub = 4;
	private int maxOption = 8;
	private bool deb_fpsIsShowing = true;
	private bool deb_enemyIsShowing = false;
	private bool deb_cmdIsShowing = true;
	private bool deb_statsIsShowing = true;
	private int height = 12;
	private bool canSelect = false;
	static public string txt_cmdOut = "";
	private bool hasOpened = false;
	private bool hasRequestedOpen = false;
	#if isDebug
	static string ver = ("Debug Build "+(BuildDate.Date)+" using Unity ");
	#else
	static 	string ver = ("Release Build "+(BuildDate.Date)+"using Unity ");
	#endif

	void Start() {
		ver += Application.unityVersion;
	#if isDebug
		OpenMenu();
	#endif
	}

	void ResetVal () {
		canSelect = false;
		selection = 4;
		noButtonPressed = true;
		submenu = false;
		selectionSub = 4;
		hasOpened = false;
		scr_main.s.SetFocus(true);
	}
	public void OpenMenu() {
		hasRequestedOpen = true;
	}
	void Open(){
		isOpen = true;
		maxOption = 8;
		ResetVal ();
		scr_main.s.SetFocus(false);
	}
	void Close(){
		isOpen = false;
		ResetVal ();
		scr_main.s.SetFocus(true);
	}

	void Update() {
		bool isPressOpen;
		bool isPressClose;
		#if UNITY_EDITOR
			bool buttonBack = Input.GetKey(KeyCode.Escape);
			bool buttonOK = Input.GetKey(KeyCode.Return);
			isPressOpen = (buttonBack && buttonOK);
			isPressClose = (buttonBack && Input.GetKey(KeyCode.LeftShift));
		#else
			bool buttonBack = UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.B);
			bool buttonOK = UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.A);
			isPressOpen = (UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.L) && UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.Start));
			isPressClose = (UnityEngine.N3DS.GamePad.GetButtonHold (N3dsButton.R) && UnityEngine.N3DS.GamePad.GetButtonHold (N3dsButton.Start));
		#endif
		if(hasRequestedOpen) { 
			isPressOpen = true;
			hasRequestedOpen = false;
		}
		if (isPressOpen) {
			Open();
		} else if (isPressClose) {
			Close();
		}
		if(isOpen){
			if(noButtonPressed && hasOpened){
				if(submenu){
					if(Input.GetKeyDown(KeyCode.UpArrow)) selectionSub--;
					if(Input.GetKeyDown(KeyCode.DownArrow)) selectionSub++;
					if(Input.GetKeyDown(KeyCode.Escape) || buttonBack) { submenu = false; selection = 3; selectionSub = 3; maxOption = 8; canSelect = false;}
				} else {
					if(Input.GetKeyDown(KeyCode.UpArrow)) selection--;
					if(Input.GetKeyDown(KeyCode.DownArrow)) selection++;
					if(Input.GetKeyDown(KeyCode.Escape) || buttonBack) { scr_main.s.SetFocus(true); isOpen = false;}
				}
	#if UNITY_EDITOR
				if(Input.GetKeyDown(KeyCode.Return) || buttonOK)
	#else
				if(buttonOK)
	#endif
				{
					switch(selection-4){
					case 0:
						isOpen = false;
						ResetVal();
						SceneManager.LoadScene ("scn_menuTitle");
						break;
					case 1:
						submenu = true;
						if (canSelect) {
							isOpen = false;
							string tmpLvName = "ERROR";
							switch (selectionSub - 4) {
							case 0:
								tmpLvName = "scn_test0";
								break;
							case 1:
								tmpLvName = "scn_test1";
								break;
							case 2:
								tmpLvName = "scn_test2";
								break;
							case 3:
								tmpLvName = "scn_capMain0";
								break;
							case 4:
								tmpLvName = "scn_globShipInside";
								break;
							case 5:
								tmpLvName = "scn_fallsMain0";
								break;
							case 6:
								tmpLvName = "scn_capExTower";
								break;
							}
							ResetVal ();
							scr_loadScene.s.StartScene (tmpLvName);
						}
						maxOption = 10;
						canSelect = true;
						break;
					case 2:
						submenu=true;
						switch(selectionSub-4){
						case 0:
							if(deb_fpsIsShowing) deb_fpsIsShowing=false; else deb_fpsIsShowing=true;
							break;
						case 1:
							if(deb_enemyIsShowing) deb_enemyIsShowing=false; else deb_enemyIsShowing=true;
							break;
						case 2:
							if (deb_cmdIsShowing) deb_cmdIsShowing = false; else deb_cmdIsShowing = true;
							break;
						case 3:
							if (deb_statsIsShowing) deb_statsIsShowing = false; else deb_statsIsShowing = true;
							break;
						}
						maxOption = 7;
						break;
					case 3:
						isOpen = false;
						ResetVal();
						scr_loadScene.s.StartScene("scn_menuTestAudio");
						break;
					case 4:
						scr_manageData.s.Save ();
						break;
					}
				}
			}
			if(!submenu){
				if(selection < 4) selection = maxOption;
				if(selection > maxOption) selection = 4;
			} else {
				if(selectionSub < 4) selectionSub = maxOption;
				if(selectionSub > maxOption) selectionSub = 4;
			}
#if UNITY_EDITOR
			if (Input.GetKeyDown(KeyCode.UpArrow)
				|| Input.GetKeyDown(KeyCode.DownArrow)
				|| Input.GetKeyDown(KeyCode.Return)
				|| Input.GetKeyDown(KeyCode.Escape))
#else
			if (UnityEngine.N3DS.GamePad.GetButtonHold (N3dsButton.A)
				|| UnityEngine.N3DS.GamePad.GetButtonHold (N3dsButton.B))
#endif
				noButtonPressed = false;
			else
			{
				noButtonPressed = true;
				hasOpened = true;
			}
		}
	}

	void DoPrint(int posN = 10, string text = "ERROR", int xOffSet = 15, int _width = 500){
		GUI.Label(new Rect(xOffSet+xoffset, ypos*posN, _width, height), text, scr_main.stl_debug);
	}

	void OnGUI(){
		if(isOpen){
			DoPrint (0, "SMO3DS DEV MENU - TEAM ALPHA'S MODDIMATION", 1);
			DoPrint (1, ver, 1);
			DoPrint (2, "Controls: A=Enter, B=Back, R+Start=Exit Menu, DPAD=Moves", 1);
			if(submenu){
				switch (selection - 4) {
				case 1:
					DoPrint (selectionSub, " > ", 1, 40);
					DoPrint (3, "Map Select", 1);
					DoPrint (4, "TestMap0-Basic", 15);
					DoPrint (5, "TestMap1-Pyramid", 15);
					DoPrint (6, "TestMap2-Camera", 15);
					DoPrint (7, "WorldCap-Main", 15);
					DoPrint (8, "Global-InOdyssey", 15);
					DoPrint (9, "WorldFalls-Main0", 15);
					DoPrint (10, "WorldCap-ExTower", 15);
					break;
				case 2:
					DoPrint (selectionSub, " > ", 1, 40);
					DoPrint (3, "Toggle Debug", 1);
					DoPrint (4, "Toggle FPS: " + deb_fpsIsShowing, 15);
					DoPrint (5, "Toggle ENEMY: " + deb_enemyIsShowing, 15);
					DoPrint (6, "Toggle CMD: " + deb_cmdIsShowing, 15);
					DoPrint (7, "Toggle STATS: " + deb_statsIsShowing, 15);
					break;
				default:
					submenu = false;
					break;
				}
			} else {
				DoPrint (selection, " > ", 1, 40);
				DoPrint (3, "Main Menu", 1);
				DoPrint (4, "Load Title Screen", 15);
				DoPrint (5, "Map Select", 15);
				DoPrint (6, "Debug Settings", 15);
				DoPrint (7, "Enter Sound Test", 15);
				DoPrint (8, "Save Game", 15);
			}
		} else {
			if(deb_fpsIsShowing) DoPrint (0, "FPS: "+1/Time.deltaTime, 1, 50);
			if(deb_enemyIsShowing) DoPrint (3, "ENEMY: "+scr_main.s.dbg_enemyCount, 1, 50);
			if(MarioController.s != null){
				DoPrint (5, "COIN: "+scr_main.s.coinsCount, 1, 50);
				DoPrint (6, "MOON: "+scr_main.s.moonsCount, 1, 50);
			}
#if !UNITY_EDITOR
		if (deb_statsIsShowing){
			DoPrint(1, "Memory: " + UnityEngine.N3DS.Debug.GetSystemFree(), 1, 50);
			DoPrint(2, "VRam A: " + UnityEngine.N3DS.Debug.GetVRAMAFree() + ", B: "+UnityEngine.N3DS.Debug.GetVRAMBFree(), 1, 50);
		}
#endif
		}
		if (deb_cmdIsShowing && txt_cmdOut != "")
			DoPrint (16, txt_cmdOut, -5, 1000);
	}
	#endif
}
